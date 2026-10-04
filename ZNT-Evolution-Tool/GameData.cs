using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using AtlasAllocator;
using ImageMagick;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZNT.Evolution.Tool;

[UsedImplicitly]
public class GameData : IDisposable
{
    // ReSharper disable once MemberCanBePrivate.Global
    public AssetsManager Manager { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public AssetsFileInstance GameRes { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public BundleFileInstance GameBundle { get; }

    private readonly IDictionary<string, MagickImage> Images = new Dictionary<string, MagickImage>();

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
        foreach (var image in Images.Values) image.Dispose();
        Images.Clear();
        GC.SuppressFinalize(this);
    }

    [UsedImplicitly]
    public IEnumerable<AssetsFileInstance> LoadAssetsFiles()
    {
        yield return GameRes;
        for (var index = 0; GameBundle.file.IsAssetsFile(index); index++)
        {
            yield return Manager.LoadAssetsFileFromBundle(GameBundle, index, true);
        }
    }

    [UsedImplicitly]
    public IEnumerable<AssetExternal> LoadTextureAtlas()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Texture2D)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_atlas")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    [UsedImplicitly]
    public IEnumerable<AssetExternal> LoadShader()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Shader)
            let fields = Manager.GetBaseField(assets, asset)
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    [UsedImplicitly]
    public IEnumerable<AssetExternal> LoadMaterial()
    {
        return
            from assets in LoadAssetsFiles()
            from asset in assets.file.GetAssetsOfType(AssetClassID.Material)
            let fields = Manager.GetBaseField(assets, asset)
            where fields["m_Name"].AsString.EndsWith("_mat")
            select new AssetExternal { file = assets, baseField = fields, info = asset };
    }

    [UsedImplicitly]
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
            let transform = Manager.GetExtAsset(assets, fields["m_Component"]["Array"][0]["component"])
            where transform.baseField["m_Father"]["m_PathID"].AsLong is 0
            select new AssetExternal { file = assets, baseField = fields, info = info };
    }

    [UsedImplicitly]
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

    [UsedImplicitly]
    public AssetExternal GetGameObject(AssetExternal asset)
    {
        return (AssetClassID)asset.info.TypeId switch
        {
            AssetClassID.GameObject => asset,
            _ => Manager.GetExtAsset(asset.file, asset.baseField["m_GameObject"]),
        };
    }

    [UsedImplicitly]
    public string GetPath(AssetExternal asset)
    {
        var o = GetGameObject(asset);
        var name = o.baseField["m_Name"].AsString;
        var transform = Manager.GetExtAsset(o.file, o.baseField["m_Component"]["Array"][0]["component"]);
        if (transform.baseField["m_Father"]["m_PathID"].AsLong is 0) return name;
        var father = Manager.GetExtAsset(transform.file, transform.baseField["m_Father"]);
        return $"{GetPath(father)}/{name}";
    }

    [UsedImplicitly]
    public MagickImage GetImage(AssetExternal texture)
    {
        var file = TextureFile.ReadTextureFile(texture.baseField);
        if (Images.TryGetValue(file.m_Name, out var source)) return source;
        source = new MagickImage(MagickColors.Transparent, (uint)file.m_Width, (uint)file.m_Height);
        source.Format = MagickFormat.Rgba;
        source.ColorSpace = ColorSpace.RGB;
        source.FilterType = FilterType.Point;
        source.Read(file.FillPictureData(texture.file), MagickFormat.Rgba);
        source.Flip();
        Images[file.m_Name] = source;

        return source;
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "SYSLIB1045")]
    public JObject? LevelElementToPixelStudio(AssetExternal element)
    {
        var asset = Manager.GetExtAsset(element.file, element.baseField["CustomAsset"]);
        if (asset.info is null) return null;
        var script = Manager.GetExtAsset(asset.file, asset.baseField["m_Script"]);
        if (script.baseField["m_ClassName"].AsString is not "HumanAsset") return null;
        var uuid = element.baseField["assetId"].AsString;
        var sprites = Manager.GetExtAsset(asset.file, asset.baseField["SpriteCollection"]);
        var animation = Manager.GetExtAsset(asset.file, asset.baseField["AnimationLibrary"]);

        var sorted = new SortedDictionary<string, AssetTypeValueField>(IdComparer.Instance);
        var name = element.baseField["m_Name"].AsString switch
        {
            "human_crs" => "human_cop_crs",
            "human_daftpunk_1" => "human_daft_punk_1",
            "human_daftpunk_2" => "human_daft_punk_2",
            "human_gunner_survivor" => "human_survivor_gunner",
            "human_perchman" => "human_soundman",
            "human_rifleman_survivor" => "human_survivor_rifleman",
            "human_shotgunner_survivor" => "human_survivor_shotgunner",
            "human_sniper" => "human_sniper_1",
            _ => element.baseField["m_Name"].AsString
        };
        var prefix = name switch
        {
            "drone" or "drone_exterminator" or "drone_invincible" or "drone_invisible" =>
                new Regex(@"^(drone_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_astrogoliath" =>
                new Regex(@"^(astrogoliath_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_astronaut" =>
                new Regex(@"^(moonsuit_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_bishop" =>
                new Regex(@"^(ash_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_boss_chemist" or "human_boss_chemist_invincible" =>
                new Regex(@"^(chemist_boss_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_boss_drug_lord" =>
                new Regex(@"^(boss1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_boss_gertrude" or "human_boss_gertrude_cinematic" =>
                new Regex(@"^(gertrude_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_bouncer" =>
                new Regex(@"^(videur_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_cheerleader" =>
                new Regex(@"^(cheerleader_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_chemist" or "human_chemist_chair" or "human_chemist_plier" =>
                new Regex(@"^(chemist_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_civilian" or "human_civilian_hostage" or "human_civilian_survivor" =>
                new Regex(@"^((?:civil_1|cicil_1)_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_civilian_black" or "human_civilian_black_explosive" =>
                new Regex(@"^(civil_3_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_civilian_dsk" =>
                new Regex(@"^(dsk_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_civilian_nude" =>
                new Regex(@"^(nudeguy_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_civilian_young" or "human_survivor_molotov" or "human_survivor_torch" =>
                new Regex(@"^((?:civil_2|civil2)_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_clown" =>
                new Regex(@"^(clown_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_cop_crs" =>
                new Regex(@"^(crs_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_cop_rifleman" or "human_cop_weak" =>
                new Regex(@"^(assault_1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_daft_punk_1" =>
                new Regex(@"^(DP1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_daft_punk_2" =>
                new Regex(@"^(DP2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_director" =>
                new Regex(@"^(director_\w+)_\d{2,}|^(soundman_\w+)_\d{2}", RegexOptions.Compiled),
            "human_driver" =>
                new Regex(@"^(driver_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_football_player" =>
                new Regex(@"^football_\w+_\d{2,}|^videur_\w+_\d{2}", RegexOptions.Compiled),
            "human_girl" or "human_girl_hostage" =>
                new Regex(@"^(girl_1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_girl_black" =>
                new Regex(@"^(girl_2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_girl_blonde" or "human_girl_blonde_garbage" =>
                new Regex(@"^(girl_3_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_girl_nude" =>
                new Regex(@"^(nudegirl_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_girl_survivor" or "human_girl_blonde_survivor" =>
                new Regex(@"^(girl_survivor_\w+)_\d{2,}|^girl_1_spawn_\d{2}", RegexOptions.Compiled),
            "human_granny" =>
                new Regex(@"^(granny_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_gunner" or "human_gunner_tutorial" =>
                new Regex(@"^((?:gunner|H_gunner)_\w+)_\d{2,}|^gunner_fall_landing_small\d{2}", RegexOptions.Compiled),
            "human_homeless" =>
                new Regex(@"^(tramp_\w+)_\d{2,}|^((?:civil_1|cicil_1)_\w+)_\d{2}", RegexOptions.Compiled),
            "human_kamikaze" =>
                new Regex(@"^(kamikaze_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_lumberjack" =>
                new Regex(@"^(chainsaw_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_machine_gunner" =>
                new Regex(@"^(minigun_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_melee" =>
                new Regex(@"^((?:batteur_1|batteur1)_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_mib_brawler" =>
                new Regex(@"^(mib2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_mib_gunner" =>
                new Regex(@"^(mib_[a-z]\w*)_\d{2,}", RegexOptions.Compiled),
            "human_mib_rifleman" =>
                new Regex(@"^(mib3_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_mib_shotgunner" =>
                new Regex(@"^(mib_4_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_ninja" =>
                new Regex(@"^(ninja_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_preacher" =>
                new Regex(@"^(preacher_\w+)_\d{2,}|^((?:civil_1|cicil_1)_\w+)_\d{2}", RegexOptions.Compiled),
            "human_priest" =>
                new Regex(@"^(priest_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_scientist_female_1" or "human_scientist_female_2" =>
                new Regex(@"(scientist_girl1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_scientist_hazmat" =>
                new Regex(@"(hazmat_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_scientist_male_1" =>
                new Regex(@"(scientist_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_scientist_male_2" =>
                new Regex(@"(scientist2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_scientist_male_3" =>
                new Regex(@"(scientist3_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_shotgunner" =>
                new Regex(@"^((?:shotgun_1|Shotgun_1)_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_sniper_1" =>
                new Regex(@"^(sniper_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_sniper_2" =>
                new Regex(@"^(sniper2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_soundman" =>
                new Regex(@"^(soundman_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_spacegirl_1" or "human_spacegirl_2" =>
                new Regex(@"^(cultist_girl_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_spaceman_1" =>
                new Regex(@"^(cultist1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_spaceman_2" =>
                new Regex(@"^(cultist2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_survivor_rifleman" =>
                new Regex(@"^(assault_survivor_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_survivor_gunner" =>
                new Regex(@"^(rick_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_survivor_shotgunner" =>
                new Regex(@"^(shotgun_survivor_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_sword_women" =>
                new Regex(@"^(sword_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_toilet_guy" =>
                new Regex(@"^(toilet_guy_\w+)_\d{2,}|^((?:civil_1|cicil_1)_\w+)_\d{2}", RegexOptions.Compiled),
            "human_virgin" =>
                new Regex(@"^(virgin_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_worker_1" =>
                new Regex(@"^(worker1_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_worker_2" =>
                new Regex(@"^(worker2_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_worker_3" =>
                new Regex(@"^(worker3_\w+)_\d{2,}", RegexOptions.Compiled),
            "human_worker_4" =>
                new Regex(@"^(worker4_\w+)_\d{2,}", RegexOptions.Compiled),
            "terminator" =>
                new Regex(@"^(terminator_\w+)_\d{2,}", RegexOptions.Compiled),
            "fake_zombie_basic" => null,
            "fake_zombie_crawler" => null,
            "fake_zombie_overlord" => null,
            "fake_zombie_tank" => null,
            "human_doctor_female" => null,
            "human_scientist_male_2_old" => null,
            _ => throw new FormatException(name)
        };
        if (prefix is null) return null;
        foreach (var definition in sprites.baseField["spriteDefinitions"]["Array"])
        {
            var id = definition["name"].AsString;
            if (prefix.IsMatch(id)) sorted[id] = definition;
        }

        foreach (var c in animation.baseField["clips"]["Array"])
        {
            if (c["name"].AsString is null or "") continue;
            foreach (var frame in c["frames"]["Array"])
            {
                if (sprites.info.PathId != frame["spriteCollection"]["m_PathID"].AsLong) continue;
                var index = frame["spriteId"].AsInt;
                var definition = sprites.baseField["spriteDefinitions"]["Array"][index];
                var id = definition["name"].AsString;
                if (prefix.IsMatch(id)) continue;
                if (id is "DP2_alert_start_03" or "sniper_aim_00") continue;
                throw new FormatException($"{name} - {c["name"].AsString} - {id}({index})");
            }
        }

        var psp = new JObject
        {
            ["Version"] = 2,
            ["Id"] = uuid,
            ["Name"] = name,
            ["Width"] = 144,
            ["Height"] = 144,
            ["Type"] = 2,
            ["Clips"] = new JArray(),
            ["Background"] = false,
            ["BackgroundColor"] = new JObject { ["r"] = 0.0, ["g"] = 0.0, ["b"] = 0.0, ["a"] = 0.0 },
            ["TileMode"] = false,
            ["TileFade"] = 50,
            ["ActiveClipIndex"] = 0
        };

        var clip = new JObject();
        foreach (var (_, definition) in sorted)
        {
            var id = definition["name"].AsString;
            var group = IdComparer.IdRegex.Match(id).Groups[1].Value;
            if (clip.Value<string>("Name") != group)
            {
                clip = new JObject
                {
                    ["Id"] = group,
                    ["Name"] = group,
                    ["Frames"] = new JArray(),
                    ["LayerTypes"] = new JArray(),
                    ["ActiveFrameIndex"] = 0
                };
                psp.Value<JArray>("Clips")!.Add(clip);
            }

            var frame = new JObject
            {
                ["Id"] = id,
                ["Delay"] = 0.1,
                ["Layers"] = new JArray(),
                ["LayerGroups"] = new JArray(),
                ["ActiveLayerIndex"] = 0
            };
            clip.Value<JArray>("Frames")!.Add(frame);

            var material = Manager.GetExtAsset(sprites.file, sprites.baseField["materials"]["Array"][0]);
            // ReSharper disable InconsistentNaming
            var m_TexEnvs = material.baseField["m_SavedProperties"]["m_TexEnvs"]["Array"];
            var _MainTex = Manager.GetExtAsset(material.file, m_TexEnvs[0][1]["m_Texture"]);
            var width = _MainTex.baseField["m_Width"].AsInt;
            var height = _MainTex.baseField["m_Height"].AsInt;
            var src_x = (int)Math.Round(
                definition["uvs"]["Array"][0]["x"].AsFloat * width - 1f / 1000f);
            var src_y = (int)Math.Round(
                (1.0f - definition["uvs"]["Array"][2]["y"].AsFloat) * height + 1f / 1000f);
            var src_w = (int)Math.Round(
                definition["uvs"]["Array"][3]["x"].AsFloat * width + 1f / 1000f - src_x);
            var src_h = (int)Math.Round(
                (1.0f - definition["uvs"]["Array"][1]["y"].AsFloat) * height - 1f / 1000f - src_y);
            var anchor_x = (int)Math.Round(
                (0.0f - definition["positions"]["Array"][0]["x"].AsFloat) / definition["texelSize"]["x"].AsFloat);
            var anchor_y = (int)Math.Round(
                definition["positions"]["Array"][2]["y"].AsFloat / definition["texelSize"]["y"].AsFloat);
            var offset_x = 60;
            var offset_y = 132;
            if (id.Contains("plier")) (offset_x, offset_y) = (72, 24);
            // ReSharper restore InconsistentNaming

            foreach (var env in m_TexEnvs)
            {
                var key = env[0].AsString;
                var source = GetImage(Manager.GetExtAsset(material.file, env[1]["m_Texture"]));
                using var patch = source.Clone();
                switch (src_w, src_h)
                {
                    case (> 0, > 0):
                        patch.Crop(new MagickGeometry(src_x, src_y, (uint)src_w, (uint)src_h));
                        break;
                    case (> 0, < 0):
                        patch.Crop(new MagickGeometry(src_x, src_y + src_h, (uint)src_w, (uint)-src_h));
                        patch.Rotate(90);
                        patch.Flip();
                        break;
                    case (-1, -1):
                        patch.Crop(new MagickGeometry(src_x + src_w, src_y + src_h, 1, 1));
                        break;
                    default:
                        throw new NotSupportedException($"src_w: {src_w}, src_h: {src_h}");
                }

                patch.Extent(new MagickGeometry(anchor_x - offset_x, anchor_y - offset_y, 144, 144));
                patch.ResetPage();

                var layer = new JObject
                {
                    ["Id"] = id + key,
                    ["Name"] = key,
                    ["Transparency"] = 1.0,
                    ["Hidden"] = key is not "_MainTex",
                    ["Linked"] = false,
                    ["Outline"] = 0,
                    ["Lock"] = 1,
                    ["Sx"] = offset_x,
                    ["Sy"] = 144 - offset_y,
                    ["Version"] = 1,
                    ["_historyJson"] = new JObject
                    {
                        ["Actions"] = new JArray(),
                        ["Index"] = 0,
                        ["_source"] = patch.ToBase64(MagickFormat.Png32)
                    }.ToString(Formatting.None)
                };
                frame.Value<JArray>("Layers")!.Add(layer);
            }

            {
                using var empty = new MagickImage(MagickColors.Transparent, 1, 1);
                foreach (var point in definition["attachPoints"]["Array"])
                {
                    var key = point["name"].AsString;
                    var x = (int)Math.Round(point["position"]["x"].AsFloat * 12);
                    var y = (int)Math.Round(point["position"]["y"].AsFloat * 12);
                    var angle = (point["angle"].AsFloat + 360) % 360;
                    var attach = new JObject
                    {
                        ["Id"] = id + "_AttachPoints(" + key + ")",
                        ["Name"] = key,
                        ["Transparency"] = angle / 360,
                        ["Hidden"] = true,
                        ["Linked"] = false,
                        ["Outline"] = 0,
                        ["Lock"] = 1,
                        ["Sx"] = x + offset_x,
                        ["Sy"] = y + 144 - offset_y,
                        ["Version"] = 1,
                        ["_historyJson"] = new JObject
                        {
                            ["Actions"] = new JArray(),
                            ["Index"] = 0,
                            ["_source"] = empty.ToBase64(MagickFormat.Png8)
                        }.ToString(Formatting.None)
                    };
                    frame.Value<JArray>("Layers")!.Add(attach);
                }
            }
        }

        return psp;
    }

    [UsedImplicitly]
    public static void PixelStudioToFiles(JObject psp, string path)
    {
        using var atlas = new MagickImage(MagickColors.Transparent, 256, 256);
        using var rim = new MagickImage(MagickColors.Transparent, 256, 256);
        var allocator = new GuillotineAtlasAllocator(new Size(256, 256));
        var name = psp.Value<string>("Name")!;
        var width = psp.Value<int>("Width")!;
        var height = psp.Value<int>("Height")!;
        var info = new JObject
        {
            ["OrthoSize"] = 0.5,
            ["TargetHeight"] = 12.0,
            ["Names"] = new JArray(),
            ["Regions"] = new JArray(),
            ["Anchors"] = new JArray(),
            ["AttachPoints"] = new JObject(),
            ["Material"] = $"sprites_{name}_mat : UnityEngine.Material",
            ["Name"] = $"sprites_{name}",
            ["Transformed"] = true
        };
        foreach (var clip in psp.Value<JArray>("Clips")!.Children<JObject>())
        {
            var part = clip.Value<string>("Name")!;
            var index = 0;
            foreach (var frame in clip.Value<JArray>("Frames")!.Children<JObject>())
            {
                // ReSharper disable once CanReplaceCastWithVariableType
                var bound = null as IMagickGeometry;
                var id = $"{part}_{index++:D2}";
                var region = new Rectangle();
                var anchor = new Point();
                foreach (var layer in frame.Value<JArray>("Layers")!.Children<JObject>())
                {
                    var key = layer.Value<string>("Name")!;
                    var history = JObject.Parse(layer.Value<string>("_historyJson")!);
                    using var source = new MagickImage(history["_source"] switch
                    {
                        JValue base64 => Convert.FromBase64String(base64.Value<string>()!),
                        JArray bytes => bytes.ToObject<byte[]>()!,
                        _ => throw new FormatException(layer.Path)
                    });
                    // ReSharper disable once InvertIf
                    if (key is "_MainTex")
                    {
                        var x = layer.Value<int>("Sx")!;
                        var y = height - layer.Value<int>("Sy")!;
                        bound = source.BoundingBox ?? new MagickGeometry(x, y, 1, 1);
                        anchor.X = x - bound.X;
                        anchor.Y = y - bound.Y;
                        var allocation = new Allocation?();
                        while (allocation is null)
                        {
                            allocation = allocator.Allocate(new Size((int)bound.Width, (int)bound.Height));
                            if (allocation is not null) break;
                            var size = allocator.Size.Width >= allocator.Size.Height
                                ? new Size(allocator.Size.Width, allocator.Size.Height * 2)
                                : new Size(allocator.Size.Width * 2, allocator.Size.Height);
                            allocator.Grow(size);
                            atlas.Extent(0, 0, (uint)size.Width, (uint)size.Height);
                            rim.Extent(0, 0, (uint)size.Width, (uint)size.Height);
                        }

                        region = allocation.Value.Rectangle;

                        info.Value<JArray>("Names")!.Add(id);
                        info.Value<JArray>("Regions")!.Add(new JObject
                        {
                            ["$type"] = "UnityEngine.Rect, UnityEngine.CoreModule",
                            ["x"] = (float)region.X,
                            ["y"] = (float)region.Y,
                            ["width"] = (float)region.Width,
                            ["height"] = (float)(height - region.Height)
                        });
                        info.Value<JArray>("Anchors")!.Add(new JObject
                        {
                            ["$type"] = "UnityEngine.Vector2, UnityEngine.CoreModule",
                            ["x"] = (float)anchor.X,
                            ["y"] = (float)(region.Height - anchor.Y)
                        });
                    }

                    switch (key)
                    {
                        case "_MainTex":
                            if (source.Width != width || source.Height != height) throw new FormatException(layer.Path);
                            source.ResetPage();
                            source.Crop(bound!);
                            atlas.Composite(source, region.X, region.Y, CompositeOperator.Replace);
                            break;
                        case "_RimTex":
                            if (source.Width != width || source.Height != height) throw new FormatException(layer.Path);
                            source.ResetPage();
                            source.Crop(bound!);
                            rim.Composite(source, region.X, region.Y, CompositeOperator.Replace);
                            break;
                        case "_FlipTex":
                            if (source.Width != width || source.Height != height) throw new FormatException(layer.Path);
                            source.ResetPage();
                            source.Crop(bound!);
                            // TODO _FlipTex
                            break;
                        default:
                            ((JArray)(info["AttachPoints"]![id] ??= new JArray())).Add(new JObject
                            {
                                ["name"] = key,
                                ["position"] = new JObject
                                {
                                    ["$type"] = "UnityEngine.Vector3, UnityEngine.CoreModule",
                                    ["x"] = (float)layer.Value<int>("Sx"),
                                    ["y"] = (float)layer.Value<int>("Sy"),
                                    ["z"] = 0.0
                                },
                                ["angle"] = layer.Value<float>("Transparency") * 360
                            });
                            break;
                    }
                }
            }
        }

        atlas.Write($@"{path}\sprites_{name}_atlas.png", MagickFormat.Png32);
        rim.Write($@"{path}\sprites_{name}_rim_atlas.png", MagickFormat.Png32);
        {
            using var fs = File.Open($@"{path}\sprites_{name}.sprite.info.json", FileMode.Create);
            using var sw = new StreamWriter(fs, Encoding.ASCII);
            using var writer = new JsonTextWriter(sw);
            sw.NewLine = "\n";
            writer.Formatting = Formatting.Indented;
            info.WriteTo(writer);
        }
    }

    [UsedImplicitly]
    internal class IdComparer : IComparer<string>
    {
        public static readonly IdComparer Instance = new();

        [SuppressMessage("Performance", "SYSLIB1045")]
        public static readonly Regex IdRegex = new(@"^(.*?)(?:_?(\d+))$", RegexOptions.Compiled);

        public int Compare(string? x, string? y)
        {
            if (x is null || y is null) return StringComparer.Ordinal.Compare(x, y);
            var xMatch = IdRegex.Match(x);
            var yMatch = IdRegex.Match(y);
            var r = StringComparer.Ordinal.Compare(xMatch.Groups[1].Value, yMatch.Groups[1].Value);
            if (r is not 0) return r;
            if (!int.TryParse(xMatch.Groups[2].Value, out var xIndex)) xIndex = 0;
            if (!int.TryParse(yMatch.Groups[2].Value, out var yIndex)) yIndex = 0;
            return xIndex.CompareTo(yIndex);
        }
    }
}