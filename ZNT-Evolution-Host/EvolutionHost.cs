using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using ZNT.Evolution.Core.Asset;

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
                ["path"] = context.Request.Url.LocalPath
            };
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
        Logger.LogInfo($"Listen: {Uri.Value}");
        try
        {
            _http = new HttpListener { IgnoreWriteExceptions = true };
            _http.Prefixes.Add(Uri.Value);
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
}