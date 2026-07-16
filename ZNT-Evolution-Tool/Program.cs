using System;
using System.Collections.Generic;
using System.Reflection;
using AssetsTools.NET.Extra;

namespace ZNT.Evolution.Tool;

internal static class Program
{
    private static string GamePath => Environment.GetEnvironmentVariable("ZNTGamePath") ?? ".";

    public static AssetsManager Manager { get; private set; }

    public static AssetsFileInstance GameRes { get; private set; }

    public static BundleFileInstance GameData { get; private set; }

    public static void Main(string[] args)
    {
        {
            Manager = new AssetsManager
            {
                MonoTempGenerator = new MonoCecilTempGenerator($"{GamePath}/znt_Data/Managed")
            };
            GameRes = Manager.LoadAssetsFile($"{GamePath}/znt_Data/Resources/unity default resources");
            using var tpk = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ZNT.Evolution.Tool.Resources.classdata.tpk");
            Manager.LoadClassPackage(tpk);
            Manager.LoadClassDatabaseFromPackage(GameRes.file.Metadata.UnityVersion);
            GameData = Manager.LoadBundleFile($"{GamePath}/znt_Data/data.unity3d");
        }

        foreach (var assets in LoadAssetsFiles())
        {
            // foreach (var shader in assets.file.GetAssetsOfType(AssetClassID.Shader))
            // {
            //     var fields = Manager.GetBaseField(assets, shader);
            //     Console.WriteLine($"| `{assets.name}` | `{shader.PathId}` | `{fields["m_ParsedForm"]["m_Name"].AsString}` |");
            // }

            // foreach (var asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour))
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var script = Manager.GetExtAsset(assets, fields["m_Script"]);
            //     var fullname = string.IsNullOrEmpty(script.baseField["m_Namespace"].AsString) 
            //         ? script.baseField["m_ClassName"].AsString
            //         : $"{script.baseField["m_Namespace"].AsString}.{script.baseField["m_ClassName"].AsString}";
            //     if (fullname != "Rotorz.Tile.OrientedBrush") continue;
            //     if (Manager.GetExtAsset(assets, fields["_orientations"][0][0]["_variations"][0][0]) is { info.TypeId: 1 } prefab)
            //     {
            //         Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{fields["m_Name"].AsString}` | `{prefab.baseField["m_Name"].AsString}` |");
            //     }
            //     else
            //     {
            //         Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{fields["m_Name"].AsString}` | |");
            //     }
            // }

            // foreach (var asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour))
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var script = Manager.GetExtAsset(assets, fields["m_Script"]);
            //     var fullname = string.IsNullOrEmpty(script.baseField["m_Namespace"].AsString) 
            //         ? script.baseField["m_ClassName"].AsString
            //         : $"{script.baseField["m_Namespace"].AsString}.{script.baseField["m_ClassName"].AsString}";
            //     if (fullname != "tk2dSpriteCollectionData") continue;
            //     var body = Manager.GetExtAsset(assets, fields["m_GameObject"]);
            //     var material = Manager.GetExtAsset(assets, fields["materials"][0][0]);
            //     var shader = Manager.GetExtAsset(assets, material.baseField["m_Shader"]);
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{body.baseField["m_Name"].AsString}` | `{shader.baseField["m_ParsedForm"]["m_Name"].AsString}` |");
            // }
        }

        {
            Manager.UnloadBundleFile(GameData);
            Manager.UnloadAssetsFile(GameRes);
        }
    }

    private static IEnumerable<AssetsFileInstance> LoadAssetsFiles()
    {
        yield return GameRes;
        for (var index = 0; GameData.file.IsAssetsFile(index); index++)
        {
            yield return Manager.LoadAssetsFileFromBundle(GameData, index, true);
        }
    }
}