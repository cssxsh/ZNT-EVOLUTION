using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Host;

public static class EvolutionHost
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(EvolutionHost));

    private static string _host = "http://localhost:8080/";

    private static HttpListener _http;

    private static string _folder = "www";

    extension(HttpListener http)
    {
        private async Task ListenAsync()
        {
            while (http.IsListening)
            {
                try
                {
                    var context = await http.GetContextAsync();
                    _ = context.HandleAsync();
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
    }

    extension(HttpListenerContext context)
    {
        private async Task HandleAsync()
        {
            try
            {
                switch (context.Request)
                {
                    case { IsWebSocketRequest: true }:
                        Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl}");
                        context.Response.Close();
                        break;
                    case { IsApi: true }:
                        // 需要读 Unity 的编辑器状态，留在主线程
                        context.HandleApi();
                        break;
                    case { HttpMethod: "GET" }:
                        await context.HandleStaticAsync();
                        break;
                    default:
                        Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl}");
                        context.Response.Close();
                        break;
                }
            }
            catch (Exception e)
            {
                // 响应已被各分支的 using 释放，这里只记录
                Logger.LogError(e);
            }
        }

        private void HandleApi()
        {
            using var response = context.Response;
            response.ContentType = "application/json; charset=utf-8";
            var obj = new JObject
            {
                ["method"] = context.Request.HttpMethod,
                ["path"] = context.Request.Url.LocalPath
            };
            try
            {
                obj["data"] = context.Request switch
                {
                    { Url.AbsolutePath: "/api/level/override", HttpMethod: "POST" } =>
                        ImportAssetOverride(context.Request),
                    { Url.AbsolutePath: "/api/level/override", HttpMethod: "GET" } =>
                        ExportAssetOverride(context.Request),
                    _ => throw new NotSupportedException("Operation not supported")
                };
                obj["status"] = 0;
                obj["message"] = "OK";
            }
            catch (Exception e)
            {
                Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl} -> {e.Message}");
                obj["status"] = -1;
                obj["message"] = e.Message;
            }

            CustomAssetUtility.SerializeObject(response.OutputStream, obj);
        }

        private async Task HandleStaticAsync()
        {
            using var response = context.Response;
            var path = $"{_folder}/{context.Request.Url.AbsolutePath}";
            if (Directory.Exists(path)) path += "/index.html";
            if (!File.Exists(path))
            {
                response.StatusCode = 404;
                return;
            }

            using var fs = File.OpenRead(path);
            response.ContentType = Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".html" or ".htm" => "text/html; charset=utf-8",
                ".css" => "text/css; charset=utf-8",
                ".js" => "application/javascript; charset=utf-8",
                ".json" => "application/json; charset=utf-8",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                ".woff" => "font/woff",
                ".woff2" => "font/woff2",
                ".ttf" => "font/ttf",
                ".txt" => "text/plain; charset=utf-8",
                _ => "application/octet-stream"
            };
            response.ContentLength64 = fs.Length;
            // 异步写出，避免在 Unity 主线程上整文件拷贝
            await fs.CopyToAsync(response.OutputStream).ConfigureAwait(false);
        }
    }

    extension(HttpListenerRequest request)
    {
        private bool IsApi => request.Url.AbsolutePath.StartsWith("/api/", StringComparison.Ordinal);
    }

    public static void Init()
    {
        _host = EvolutionHostPlugin.Uri.Value;
        _folder = $"{Paths.PluginPath}/ZNT-Evolution-Host/www_{EvolutionHostPlugin.Instance.Info.Metadata.Version}";
        // ReSharper disable once InvertIf
        if (!Directory.Exists(_folder))
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (!resource.StartsWith("www", StringComparison.Ordinal)) continue;
                var path = $"{Paths.PluginPath}/ZNT-Evolution-Host/{resource}";
                using var source = assembly
                    .GetManifestResourceStream(resource) ?? throw new FileNotFoundException(resource);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var output = File.Create(path);
                source.CopyTo(output);
            }
        }
    }

    public static void Start()
    {
        if (_http is { IsListening: true }) return;
        Logger.LogInfo($"Listen: {_host}");
        try
        {
            _http = new HttpListener { IgnoreWriteExceptions = true };
            _http.Prefixes.Add(_host);
            _http.Start();
            _ = _http.ListenAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            _http?.Close();
            _http = null;
        }
    }

    public static void Stop()
    {
        if (_http is null) return;
        try
        {
            _http.Stop();
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
        finally
        {
            _http.Close();
            _http = null;
        }
    }

    [UsedImplicitly]
    private static JToken ImportAssetOverride(HttpListenerRequest request)
    {
        if (!ComponentSingleton<LevelSettings>.HasInstance()) return new JArray();
        EvolutionSettings.Instance.AssetOverrideData =
            CustomAssetUtility.DeserializeObject<List<EvolutionSettings.AssetOverrideRecord>>(request.InputStream);
        CustomAssetUtility.SerializeObject(out var token, EvolutionSettings.Instance.AssetOverrideData);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportAssetOverride(HttpListenerRequest request)
    {
        if (!ComponentSingleton<LevelSettings>.HasInstance()) return new JArray();
        CustomAssetUtility.SerializeObject(out var token, EvolutionSettings.Instance.AssetOverrideData);
        return token;
    }
}