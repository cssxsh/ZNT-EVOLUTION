using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using ZNT.Evolution.Core.Asset;
using ZNT.Evolution.Core.Editor;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Host;

public static class EvolutionHost
{
    private static HttpListener _http;

    internal static ManualLogSource Logger;

    internal static ConfigEntry<string> Uri;

    internal static string RootFolder = ".";

    extension(HttpListener http)
    {
        private async Task ListenAsync()
        {
            while (http.IsListening)
            {
                try
                {
                    var context = await http.GetContextAsync();
                    context.Handle();
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
        private void Handle()
        {
            switch (context.Request)
            {
                case { IsWebSocketRequest: true }:
                    Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl}");
                    context.Response.Close();
                    break;
                case { IsApi: true }:
                    context.HandleApi();
                    break;
                case { HttpMethod: "GET" }:
                    context.HandleStatic();
                    break;
                default:
                    Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl}");
                    context.Response.Close();
                    break;
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
                Logger.LogWarning(e);
                obj["status"] = -1;
                obj["message"] = e.Message;
            }

            CustomAssetUtility.SerializeObject(response.OutputStream, obj);
        }

        private void HandleStatic()
        {
            using var response = context.Response;
            var path = $"{RootFolder}/{context.Request.Url.AbsolutePath}";
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
            fs.CopyTo(response.OutputStream);
        }
    }

    extension(HttpListenerRequest request)
    {
        private bool IsApi => request.Url.AbsolutePath.StartsWith("/api/", StringComparison.Ordinal);
    }

    public static void Start()
    {
        if (_http is { IsListening: true }) return;
        var host = Uri?.Value ?? "http://localhost:8080/";
        Logger ??= BepInExLogger.CreateLogSource("Evolution Host");
        Logger.LogInfo($"Listen: {host}");
        try
        {
            _http = new HttpListener { IgnoreWriteExceptions = true };
            _http.Prefixes.Add(host);
            _http.Start();
            _ = _http.ListenAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            _http.Close();
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