using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace ZNT.Evolution.Fuck;

public static class SteamManagerPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp.dll"];

    public static void Patch(AssemblyDefinition assembly)
    {
        var _SteamManager = assembly.MainModule.GetType("SteamManager");
        var _DeleteSteamAppId = _SteamManager.Methods.First(method => method.Name == "DeleteSteamAppId");
        // Application.Quit();
        // _DeleteSteamAppId.Body.Instructions[13] = Instruction.Create(OpCodes.Nop);
        _DeleteSteamAppId.Body.ExceptionHandlers.Clear();
        _DeleteSteamAppId.Body.Instructions.Clear();
        _DeleteSteamAppId.Body.Variables.Clear();
        _DeleteSteamAppId.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
    }
}