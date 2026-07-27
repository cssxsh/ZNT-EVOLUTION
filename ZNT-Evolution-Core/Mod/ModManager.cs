using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using JetBrains.Annotations;
using BepInExLogger = BepInEx.Logging.Logger;

namespace ZNT.Evolution.Core.Mod;

public static class ModManager
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(ModManager));

    static ModManager() => Encoding.RegisterProvider(new Fix437EncodingProvider(Encoding.ASCII));

    private class Fix437EncodingProvider(Encoding fallback) : EncodingProvider
    {
        public override Encoding GetEncoding(string name) => name is "IBM437" ? fallback : null;
        public override Encoding GetEncoding(int codepage) => codepage is 437 ? fallback : null;
    }

    [UsedImplicitly]
    public static async Task LoadAll()
    {
        var mods = Path.Combine(UnityEngine.Application.dataPath, "Mods");
        if (!Directory.Exists(mods)) Directory.CreateDirectory(mods);
        var allocated = new List<ModContext>();

        foreach (var folder in Directory.EnumerateDirectories(mods))
        {
            Logger.LogDebug($"allocate context from folder '{folder}'");
            try
            {
                allocated.Add(ModContext.Allocate(folder));
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
            }
        }

        foreach (var package in Directory.EnumerateFiles(mods, "*.zip"))
        {
            Logger.LogDebug($"allocate context from package '{package}'");
            try
            {
                allocated.Add(ModContext.Allocate(package));
            }
            catch (System.Exception e)
            {
                Logger.LogError(e);
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
}