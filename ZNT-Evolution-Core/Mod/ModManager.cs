using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using JetBrains.Annotations;
using Steamworks;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Mod;

public static class ModManager
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(ModManager));

    public static string ModsPath = System.Environment.GetEnvironmentVariable("ZNTModsPath");

    static ModManager() => Encoding.RegisterProvider(new Fix437EncodingProvider(Encoding.ASCII));

    private class Fix437EncodingProvider(Encoding fallback) : EncodingProvider
    {
        public override Encoding GetEncoding(string name) => name is "IBM437" ? fallback : null;
        public override Encoding GetEncoding(int codepage) => codepage is 437 ? fallback : null;
    }

    [UsedImplicitly]
    public static async Task LoadAll()
    {
        var allocated = new List<ModContext>();

        foreach (var folder in ModsFolder())
        {
            Logger.LogDebug($"find mod from '{folder}'");
            foreach (var path in Directory.EnumerateFileSystemEntries(folder))
            {
                if (path.EndsWith(".zip") || path.EndsWith(".mod"))
                {
                    Logger.LogDebug($"allocate context from package '{path}'");
                }
                else if (Directory.Exists(path) && File.Exists($"{path}/metadata.json"))
                {
                    Logger.LogDebug($"allocate context from folder '{path}'");
                }
                else
                {
                    continue;
                }

                try
                {
                    allocated.Add(ModContext.Allocate(path));
                }
                catch (System.Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }

        while (allocated.Find(context => context.IsLoadReady()) is { } mod)
        {
            try
            {
                await mod.Load();
                allocated.Remove(mod);
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
        }
    }

    [UsedImplicitly]
    public static async Task UnloadAll()
    {
        var allocated = ModContext.Allocated().ToList();

        while (allocated.Find(context => context.IsUnloadReady()) is { } mod)
        {
            try
            {
                await mod.Unload();
                ModContext.Free(mod.Metadata.Id);
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
            finally
            {
                allocated.Remove(mod);
            }
        }
    }

    [UsedImplicitly]
    public static async Task ReloadAll()
    {
        await UnloadAll();
        await LoadAll();
    }

    [UsedImplicitly]
    private static IEnumerable<string> ModsFolder()
    {
        if (Directory.Exists(ModsPath)) yield return ModsPath;

        var count = SteamUGC.GetNumSubscribedItems();
        var ids = new PublishedFileId_t[count];
        _ = SteamUGC.GetSubscribedItems(ids, count);
        foreach (var id in ids)
        {
            _ = SteamUGC.GetItemInstallInfo(id, out _, out var folder, 1024U, out _);
            if (folder is null) continue;
            var path = $"{folder}/Mods";
            if (Directory.Exists(path)) yield return path;
        }
    }
}