using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AssetsTools.NET.Extra;

namespace ZNT.Evolution.Tool;

internal static class Program
{
    private static string GamePath => Environment.GetEnvironmentVariable("ZNTGamePath") ?? ".";

    // ReSharper disable once MemberCanBePrivate.Global
    public static AssetsManager Manager { get; private set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public static AssetsFileInstance GameRes { get; private set; }

    // ReSharper disable once MemberCanBePrivate.Global
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

        {
            Manager.UnloadBundleFile(GameData);
            Manager.UnloadAssetsFile(GameRes);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetsFileInstance> LoadAssetsFiles()
    {
        yield return GameRes;
        for (var index = 0; GameData.file.IsAssetsFile(index); index++)
        {
            yield return Manager.LoadAssetsFileFromBundle(GameData, index, true);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private static AssetExternal GetGameObject(this AssetExternal asset)
    {
        return (AssetClassID)asset.info.TypeId switch
        {
            AssetClassID.GameObject => asset,
            _ => Manager.GetExtAsset(asset.file, asset.baseField["m_GameObject"]),
        };
    }

    // ReSharper disable once UnusedMember.Local
    private static string GetPath(this AssetExternal asset)
    {
        var o = asset.GetGameObject();
        var name = o.baseField["m_Name"].AsString;
        var transform = Manager.GetExtAsset(o.file, o.baseField["m_Component.Array"][0]["component"]);
        if (transform.baseField["m_Father"]["m_PathID"].AsLong is 0) return name;
        var father = Manager.GetExtAsset(transform.file, transform.baseField["m_Father"]);
        return $"{father.GetPath()}/{name}";
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadTexture2D()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Texture2D)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_atlas")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadShader()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Shader)
            let fields = Manager.GetBaseField(assets, asset)
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadMaterial()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Material)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_mat")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadPrefab()
    {
        return
            from assets in LoadAssetsFiles()
            where !assets.name.StartsWith("level")
            from info in assets.file.GetAssetsOfType(AssetClassID.GameObject)
            let fields = Manager.GetBaseField(assets, info)
            let name = fields["m_Name"].AsString
            where !(
                name.StartsWith("sprites_") ||
                name.StartsWith("sprite_") ||
                name.StartsWith("anim_"))
            let transform = Manager.GetExtAsset(assets, fields["m_Component.Array"][0]["component"])
            where transform.baseField["m_Father"]["m_PathID"].AsLong is 0
            select new AssetExternal { file = assets, baseField = fields, info = info };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadSprites()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == "tk2dSpriteCollectionData"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadAnimation()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == "tk2dSpriteAnimation"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadVisualEffect()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == "VisualEffect"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadMonoBehaviour(string type)
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString == type
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadAssetElement()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
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
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<AssetExternal> LoadTMProAsset()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_Namespace"].AsString is "TMPro" &&
                  script.baseField["m_ClassName"].AsString is "TMP_FontAsset" or "TMP_SpriteAsset"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }
}