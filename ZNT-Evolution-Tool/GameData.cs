using System.Reflection;
using AssetsTools.NET.Extra;

namespace ZNT.Evolution.Tool;

public class GameData : IDisposable
{
    // ReSharper disable once MemberCanBePrivate.Global
    public AssetsManager Manager { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public AssetsFileInstance GameRes { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public BundleFileInstance GameBundle { get; }

    public GameData(string path)
    {
        Manager = new AssetsManager
        {
            MonoTempGenerator = new MonoCecilTempGenerator($"{path}/znt_Data/Managed")
        };
        GameRes = Manager.LoadAssetsFile($"{path}/znt_Data/Resources/unity default resources");
        GameBundle = Manager.LoadBundleFile($"{path}/znt_Data/data.unity3d");
        using var tpk =
            Assembly.GetExecutingAssembly().GetManifestResourceStream("ZNT.Evolution.Tool.Resources.classdata.tpk")
            ?? throw new FileNotFoundException("classdata.tpk");
        Manager.LoadClassPackage(tpk);
        Manager.LoadClassDatabaseFromPackage(GameBundle.file.Header.EngineVersion);
    }

    public void Dispose()
    {
        Manager.UnloadBundleFile(GameBundle);
        Manager.UnloadAssetsFile(GameRes);
        GC.SuppressFinalize(this);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public IEnumerable<AssetsFileInstance> LoadAssetsFiles()
    {
        yield return GameRes;
        for (var index = 0; GameBundle.file.IsAssetsFile(index); index++)
        {
            yield return Manager.LoadAssetsFileFromBundle(GameBundle, index, true);
        }
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadTexture2D()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Texture2D)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_atlas")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadShader()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Shader)
            let fields = Manager.GetBaseField(assets, asset)
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadMaterial()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Material)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_mat")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadPrefab()
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
    public IEnumerable<AssetExternal> LoadSprites()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString is "tk2dSpriteCollectionData"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadAnimation()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString is "tk2dSpriteAnimation"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadVisualEffect()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.MonoBehaviour)
            let fields = Manager.GetBaseField(assets, asset)
            let script = Manager.GetExtAsset(assets, fields["m_Script"])
            where script.baseField["m_ClassName"].AsString is "VisualEffect"
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    // ReSharper disable once UnusedMember.Local
    public IEnumerable<AssetExternal> LoadMonoBehaviour(string type)
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
    public IEnumerable<AssetExternal> LoadAssetElement()
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
    public IEnumerable<AssetExternal> LoadTMProAsset()
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

    // ReSharper disable once MemberCanBePrivate.Global
    public AssetExternal GetGameObject(AssetExternal asset)
    {
        return (AssetClassID)asset.info.TypeId switch
        {
            AssetClassID.GameObject => asset,
            _ => Manager.GetExtAsset(asset.file, asset.baseField["m_GameObject"]),
        };
    }

    // ReSharper disable once UnusedMember.Local
    public string GetPath(AssetExternal asset)
    {
        var o = GetGameObject(asset);
        var name = o.baseField["m_Name"].AsString;
        var transform = Manager.GetExtAsset(o.file, o.baseField["m_Component.Array"][0]["component"]);
        if (transform.baseField["m_Father"]["m_PathID"].AsLong is 0) return name;
        var father = Manager.GetExtAsset(transform.file, transform.baseField["m_Father"]);
        return $"{GetPath(father)}/{name}";
    }
}