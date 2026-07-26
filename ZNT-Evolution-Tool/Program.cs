using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AssetsTools.NET;
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
            // foreach (var asset in assets.LoadTexture2D())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var name = fields["m_Name"].AsString;
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name} : UnityEngine.Texture2D` |");
            // }
            //
            // foreach (var asset in assets.LoadShader())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var name = fields["m_ParsedForm"]["m_Name"].AsString;
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name}` |");
            // }
            //
            // foreach (var asset in assets.LoadMaterial())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var name = fields["m_Name"].AsString;
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name}` |");
            // }
            //
            // foreach (var asset in assets.LoadSprites())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var name = Manager.GetExtAsset(assets, fields["m_GameObject"]).baseField["m_Name"].AsString;
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name} : tk2dSpriteCollectionData` |");
            // }
            //
            // foreach (var asset in assets.LoadAnimation())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var name = Manager.GetExtAsset(assets, fields["m_GameObject"]).baseField["m_Name"].AsString;
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name} : tk2dSpriteAnimation` |");
            // }
            //
            // foreach (var asset in assets.LoadAssetElement())
            // {
            //     var fields = Manager.GetBaseField(assets, asset);
            //     var script = Manager.GetExtAsset(assets, fields["m_Script"]);
            //     var name = fields["m_Name"].AsString;
            //     var type = script.baseField["m_ClassName"].AsString;
            //     // if (script.baseField["m_Namespace"].AsString != "") throw new Exception(type);
            //     Console.WriteLine($"| `{assets.name}` | `{asset.PathId}` | `{name} : {type}` |");
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
    
    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadTexture2D(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.Texture2D)
            let fields = Manager.GetBaseField(assets, asset)
            let name = fields["m_Name"].AsString
            where name.EndsWith("_atlas")
            select asset;
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadShader(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.Shader)
            let fields = Manager.GetBaseField(assets, asset)
            // where fields["m_ParsedForm"]["m_Name"].AsString.StartsWith("ZNT/")
            select asset;
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadMaterial(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.Material)
            let fields = Manager.GetBaseField(assets, asset)
            let name = fields["m_Name"].AsString
            where name.EndsWith("_mat")
            select asset;
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadSprites(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == "tk2dSpriteCollectionData"
            select asset;
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadAnimation(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == "tk2dSpriteAnimation"
            select asset;
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetFileInfo> LoadAssetElement(this AssetsFileInstance assets)
    {
        return from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString is
                "AssetElement" or
                "FMODAsset" or
                "AssetElementIndex" or
                "CustomAssetObject" or
                "GameConfAsset" or
                "PoolSettingsAsset" or
                "InputAsset" or
                "FmodAssetIndex" or
                "CharacterAnimationAsset" or
                "CharacterAsset" or
                "CharacterSoundAsset" or
                "HumanAsset" or
                "SentryGunAsset" or
                "WorldEnemyAsset" or
                "ZombieAsset" or
                "DetectionAsset" or
                "ScreamAsset" or
                "BreakablePropAsset" or
                "DecorAsset" or
                "ExplosionAsset" or
                "MovingObjectAsset" or
                "MutationsConfigAsset" or
                "PhysicObjectAsset" or
                "TriggerAsset" or
                "ShaderAnimator" or
                "ShaderAnimatorIndex" or
                "VisualEffect" or
                "VisualEffectIndex" or
                "LevelElement" or
                "LevelElementIndex" or
                "BlockerMutation" or
                "BoomerMutation" or
                "CharacterMutation" or
                "ClimberMutation" or
                "ContaminationMutation" or
                "JumpMutation" or
                "RunnerMutation" or
                "SacrificeMutation" or
                "ScreamerMutation" or
                "SpitMutation" or
                "TankMutation"
            select asset;
    }
}