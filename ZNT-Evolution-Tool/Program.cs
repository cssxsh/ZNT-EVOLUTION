using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AssetsTools.NET.Extra;

namespace ZNT.Evolution.Tool;

internal static class Program
{
    private static string GamePath => Environment.GetEnvironmentVariable("ZNTGamePath") ?? ".";

    public static void Main(string[] args)
    {
        var manager = new AssetsManager
        {
            MonoTempGenerator = new MonoCecilTempGenerator($"{GamePath}/znt_Data/Managed")
        };
        if (manager.LoadAssetsFile($"{GamePath}/znt_Data/Resources/unity default resources") is
            { file.Metadata.TypeTreeEnabled: false } incomplete)
        {
            using var tpk = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ZNT.Evolution.Tool.Resources.classdata.tpk");
            manager.LoadClassPackage(tpk);
            manager.LoadClassDatabaseFromPackage(incomplete.file.Metadata.UnityVersion);
        }

        var bundle = manager.LoadBundleFile($"{GamePath}/znt_Data/data.unity3d");

        // var resources = manager.LoadAssetsFileFromBundle(bundle, "resources.assets", true);
        // foreach (var asset in resources.file.GetAssetsOfType(AssetClassID.MonoBehaviour))
        // {
        //     var goBase = manager.GetBaseField(resources, asset);
        //     var script = manager.GetExtAsset(resources, goBase["m_Script"]);
        //     Console.WriteLine(script.baseField["m_Name"].AsString);
        // }
    }
}