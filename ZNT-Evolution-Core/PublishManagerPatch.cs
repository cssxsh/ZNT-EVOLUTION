using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using ZNT.Evolution.Core.Mod;
using ZNT.LevelEditor;
using BepInExLogger = BepInEx.Logging.Logger;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class PublishManagerPatch
{
    private static readonly ManualLogSource Logger = BepInExLogger.CreateLogSource(nameof(PublishManager));

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PublishManager), "StartLocalPublish")]
    public static void StartLocalPublish(Chapter chapter, bool temp)
    {
        if (!(temp && LevelManager.Mode is LevelManager.SourceMode.EditorCustomLevels)) return;
        if (chapter.SteamWorkshopId is 0) return;
        var folder = $"{LevelManager.GetChaptersPath(LevelManager.SourceMode.Temp, true)}/Mods";
        var steam_link = $"steam://url/CommunityFilePage/{chapter.SteamWorkshopId}";
        var web_link = $"https://steamcommunity.com/sharedfiles/filedetails/?id={chapter.SteamWorkshopId}";
        if (Directory.Exists(folder)) Directory.Delete(folder, true);
        foreach (var context in ModContext.Allocated()
                     .Where(context => context.Metadata.Link == steam_link || context.Metadata.Link == web_link))
        {
            Logger.LogInfo($"Copy {context.Path} to {folder}");
            foreach (var file in context.Files())
            {
                var dest = folder + file.Substring(context.Path.Length);
                var directory = Path.GetDirectoryName(dest)!;
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                File.Copy(file, dest);
            }
        }
    }

    private static IEnumerable<string> Files(this ModContext context)
    {
        if (File.Exists(context.Path))
        {
            yield return context.Path;
            yield break;
        }

        foreach (var file in Directory.EnumerateFiles(context.Path, "*", SearchOption.AllDirectories))
        {
            yield return file;
        }
    }
}