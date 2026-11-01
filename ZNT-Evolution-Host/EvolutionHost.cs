using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;
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
                obj["status"] = 0;
                obj["message"] = "OK";
                obj["data"] = context.Request.HttpMethod switch
                {
                    "GET" => context.Request.Url.AbsolutePath switch
                    {
                        "/api/data/material" => ExportMaterial(context.Request),
                        "/api/data/animation" => ExportAnimation(context.Request),
                        "/api/data/explosion" => ExportExplosionAsset(context.Request),
                        "/api/data/physic" => ExportPhysicObjectAsset(context.Request),
                        "/api/data/human" => ExportHumanAsset(context.Request),
                        "/api/data/element" => ExportLevelElement(context.Request),
                        "/api/level/override" => ExportAssetOverride(context.Request),
                        _ => throw new NotSupportedException("Operation not supported")
                    },
                    "POST" => context.Request.Url.AbsolutePath switch
                    {
                        "/api/level/override" => ImportAssetOverride(context.Request),
                        _ => throw new NotSupportedException("Operation not supported")
                    },
                    _ => throw new NotSupportedException("Operation not supported")
                };
            }
            catch (Exception e)
            {
                obj["status"] = -1;
                obj["message"] = e.Message;
                Logger.LogWarning($"{context.Request.HttpMethod}: {context.Request.RawUrl} -> {e.Message}");
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
        _folder = $"{Paths.PluginPath}/ZNT-Evolution-Host/www";
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
    [SuppressMessage("ReSharper", "Unity.PreferAddressByIdToGraphicsParams")]
    private static JToken ExportMaterial(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var material = CustomAssetUtility.DeserializeObject<Material>($"{name} : UnityEngine.Material");
        if (material is null) return null;
        return new JObject
        {
            ["Name"] = material.name,
            ["Shader"] = material.shader.name,
            ["Textures"] = material.shader.name switch
            {
                "ZNT/Characters/Characters Base" => new JObject
                {
                    ["_MainTex"] = material.GetTexture("_MainTex")?.NameAndType(),
                    ["_RimTex"] = material.GetTexture("_RimTex")?.NameAndType()
                },
                "ZNT/Evolution/Flat Textured AlphaTest" => new JObject
                {
                    ["_MainTex"] = material.GetTexture("_MainTex")?.NameAndType(),
                    ["_FlipTex"] = material.GetTexture("_FlipTex")?.NameAndType()
                },
                _ => new JObject
                {
                    ["_MainTex"] = material.GetTexture("_MainTex")?.NameAndType()
                }
            },
            ["Floats"] = material.shader.name switch
            {
                "ZNT/Characters/Characters Base" => new JObject
                {
                    ["_AmbientModifier"] = material.GetFloat("_AmbientModifier"),
                    // ["_AmbientToggle"] = material.GetFloat("_AmbientToggle"),
                    ["_Brightness"] = material.GetFloat("_Brightness"),
                    ["_CullingType"] = material.GetFloat("_CullingType"),
                    ["_Cutoff"] = material.GetFloat("_Cutoff"),
                    ["_MainTexHSize"] = material.GetFloat("_MainTexHSize"),
                    ["_MainTexVSize"] = material.GetFloat("_MainTexVSize"),
                    ["_RimIntensity"] = material.GetFloat("_RimIntensity"),
                    ["_RimSize"] = material.GetFloat("_RimSize"),
                    ["_SelfIllumination"] = material.GetFloat("_SelfIllumination"),
                    // ["_VertexColorIntensity"] = material.GetFloat("_VertexColorIntensity"),
                    // ["_Limit"] = material.GetFloat("_Limit"),
                    // ["_Parallax"] = material.GetFloat("_Parallax"),
                    // ["_SpecularPower"] = material.GetFloat("_SpecularPower")
                },
                "ZNT/Common/Flat Textured AlphaTest" => new JObject
                {
                    // ["_Brightness"] = material.GetFloat("_Brightness"),
                    // ["_CullingType"] = material.GetFloat("_CullingType"),
                    // ["_Cutoff"] = material.GetFloat("_Cutoff"),
                    // ["_MainTexHSize"] = material.GetFloat("_MainTexHSize"),
                    // ["_MainTexVSize"] = material.GetFloat("_MainTexVSize"),
                    // ["_RimIntensity"] = material.GetFloat("_RimIntensity"),
                    // ["_RimSize"] = material.GetFloat("_RimSize"),
                    // ["_SelfIllumination"] = material.GetFloat("_SelfIllumination"),
                    // ["_VertexColorIntensity"] = material.GetFloat("_VertexColorIntensity")
                },
                "ZNT/Common/Animated Flat Textured Cutout" => new JObject
                {
                    ["_ScaleX"] = material.GetFloat("_ScaleX"),
                    ["_ScaleY"] = material.GetFloat("_ScaleY"),
                    ["_Speed"] = material.GetFloat("_Speed")
                },
                "ZNT/Common/Animated Flat Textured Transparent" => new JObject
                {
                    ["_BlendType1"] = material.GetFloat("_BlendType1"),
                    ["_BlendType2"] = material.GetFloat("_BlendType2"),
                    // ["_BumpScale"] = material.GetFloat("_BumpScale"),
                    ["_CullingType"] = material.GetFloat("_CullingType"),
                    ["_CutOut"] = material.GetFloat("_CutOut"),
                    // ["_Cutoff"] = material.GetFloat("_Cutoff"),
                    // ["_DetailNormalMapScale"] = material.GetFloat("_DetailNormalMapScale"),
                    // ["_DstBlend"] = material.GetFloat("_DstBlend"),
                    // ["_EmissivePower"] = material.GetFloat("_EmissivePower"),
                    // ["_Glossiness"] = material.GetFloat("_Glossiness"),
                    // ["_Metallic"] = material.GetFloat("_Metallic"),
                    // ["_Mode"] = material.GetFloat("_Mode"),
                    // ["_OcclusionStrength"] = material.GetFloat("_OcclusionStrength"),
                    // ["_Parallax"] = material.GetFloat("_Parallax"),
                    ["_ScaleX"] = material.GetFloat("_ScaleX"),
                    ["_ScaleY"] = material.GetFloat("_ScaleY"),
                    // ["_SpecularPower"] = material.GetFloat("_SpecularPower"),
                    ["_Speed"] = material.GetFloat("_Speed"),
                    // ["_SrcBlend"] = material.GetFloat("_SrcBlend"),
                    // ["_UVSec"] = material.GetFloat("_UVSec"),
                    ["_ZWrite"] = material.GetFloat("_ZWrite")
                },
                "ZNT/Environments/Environments Base" => new JObject
                {
                    // ["_AmbientModifier"] = material.GetFloat("_AmbientModifier"),
                    // ["_Brightness"] = material.GetFloat("_Brightness"),
                    // ["_CullingType"] = material.GetFloat("_CullingType"),
                    ["_Cutoff"] = material.GetFloat("_Cutoff"),
                    ["_EmissivePower"] = material.GetFloat("_EmissivePower"),
                    // ["_SelfIllumination"] = material.GetFloat("_SelfIllumination"),
                    ["_SpecularPower"] = material.GetFloat("_SpecularPower"),
                    // ["_RimIntensity"] = material.GetFloat("_RimIntensity"),
                    // ["_MainTexHSize"] = material.GetFloat("_MainTexHSize"),
                    // ["_MainTexVSize"] = material.GetFloat("_MainTexVSize"),
                    // ["_RimSize"] = material.GetFloat("_RimSize"),
                    // ["PixelSnap"] = material.GetFloat("PixelSnap"),
                    // ["_BlendType1"] = material.GetFloat("_BlendType1"),
                    // ["_BlendType2"] = material.GetFloat("_BlendType2"),
                    // ["_ZWrite"] = material.GetFloat("_ZWrite"),
                    // ["_ScaleX"] = material.GetFloat("_ScaleX"),
                    // ["_ScaleY"] = material.GetFloat("_ScaleY"),
                    // ["_Speed"] = material.GetFloat("_Speed"),
                    // ["_BumpScale"] = material.GetFloat("_BumpScale"),
                    // ["_DetailNormalMapScale"] = material.GetFloat("_DetailNormalMapScale"),
                    // ["_DstBlend"] = material.GetFloat("_DstBlend"),
                    // ["_Glossiness"] = material.GetFloat("_Glossiness"),
                    // ["_Metallic"] = material.GetFloat("_Metallic"),
                    // ["_Mode"] = material.GetFloat("_Mode"),
                    // ["_OcclusionStrength"] = material.GetFloat("_OcclusionStrength"),
                    // ["_Parallax"] = material.GetFloat("_Parallax"),
                    // ["_SrcBlend"] = material.GetFloat("_SrcBlend"),
                    // ["_UVSec"] = material.GetFloat("_UVSec")
                },
                "ZNT/Evolution/Flat Textured AlphaTest" => new JObject
                {
                    ["_UseFlip"] = material.GetFloat("_UseFlip"),
                    ["_AmbientValue"] = material.GetFloat("_AmbientValue")
                },
                _ => new JObject()
            },
            ["Colors"] = material.shader.name switch
            {
                _ => new JObject
                {
                    ["_Color"] = JObject.FromObject(material.GetColor("_Color"), CustomAssetUtility.Serializer)
                }
            }
        };
    }

    [UsedImplicitly]
    private static JToken ExportAnimation(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var animation = CustomAssetUtility.DeserializeObject<tk2dSpriteAnimation>($"{name} : tk2dSpriteAnimation");
        CustomAssetUtility.SerializeObject(out var token, animation);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportExplosionAsset(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var explosion = CustomAssetUtility.DeserializeObject<ExplosionAsset>($"{name} : ExplosionAsset");
        CustomAssetUtility.SerializeObject(out var token, explosion);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportPhysicObjectAsset(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var physic = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>($"{name} : PhysicObjectAsset");
        CustomAssetUtility.SerializeObject(out var token, physic);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportHumanAsset(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var human = CustomAssetUtility.DeserializeObject<HumanAsset>($"{name} : HumanAsset");
        CustomAssetUtility.SerializeObject(out var token, human);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportLevelElement(HttpListenerRequest request)
    {
        var name = request.QueryString.Get("name") ?? throw new ArgumentException("need name");
        var element = CustomAssetUtility.DeserializeObject<LevelElement>($"{name} : LevelElement");
        CustomAssetUtility.SerializeObject(out var token, element);
        return token;
    }

    [UsedImplicitly]
    private static JToken ExportAssetOverride(HttpListenerRequest request)
    {
        if (!ComponentSingleton<LevelSettings>.HasInstance()) return new JArray();
        CustomAssetUtility.SerializeObject(out var token, EvolutionSettings.Instance.AssetOverrideData);
        return token;
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
}