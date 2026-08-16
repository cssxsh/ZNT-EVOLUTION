using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace ZNT.Evolution.Fuck;

public static class SteamworksPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["Assembly-CSharp-firstpass.dll"];

    public static void Patch(AssemblyDefinition assembly)
    {
        var T_CCallbackBaseVTable = assembly.MainModule.GetType("Steamworks.CCallbackBaseVTable");
        PatchVTable(T_CCallbackBaseVTable);
        var T_CCallback = assembly.MainModule.GetType("Steamworks.Callback`1");
        PatchCall(T_CCallback);
        var T_CallResult = assembly.MainModule.GetType("Steamworks.CallResult`1");
        PatchCall(T_CallResult);
    }

    private static void PatchVTable(TypeDefinition type)
    {
        var P_pThis = new ParameterDefinition(
            "pThis",
            ParameterAttributes.None,
            type.Module.TypeSystem.IntPtr);
        foreach (var @delegate in type.NestedTypes)
        {
            // [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            var A_UnmanagedFunctionPointer =
                @delegate.GetCustomAttribute(typeof(UnmanagedFunctionPointerAttribute).FullName);
            if (A_UnmanagedFunctionPointer == null) continue;
            A_UnmanagedFunctionPointer.ConstructorArguments[0] =
                new CustomAttributeArgument(A_UnmanagedFunctionPointer.ConstructorArguments[0].Type, 4);
            // Invoke
            @delegate.Methods[1].Parameters.Insert(0, P_pThis);
            // BeginInvoke
            @delegate.Methods[2].Parameters.Insert(0, P_pThis);
        }
    }

    private static void PatchCall(TypeDefinition type)
    {
        var P_pCCallbackBase = new ParameterDefinition(
            "pCCallbackBase",
            ParameterAttributes.None,
            type.Module.TypeSystem.IntPtr);
        foreach (var method in type.Methods.Where(method => method.Name is
                     "OnRunCallback" or
                     "OnRunCallResult" or
                     "OnGetCallbackSizeBytes" or
                     "BuildCCallbackBase"))
        {
            if (method.Name.StartsWith("On")) method.Parameters.Insert(0, P_pCCallbackBase);
            foreach (var instruction in method.Body.Instructions)
            {
                switch (instruction)
                {
                    case { OpCode.Code: Code.Ldarg_1 }:
                        instruction.OpCode = OpCodes.Ldarg_2;
                        break;
                    case { OpCode.Code: Code.Ldarg_2 }:
                        instruction.OpCode = OpCodes.Ldarg_3;
                        break;
                    case { OpCode.Code: Code.Ldarg_3 }:
                        instruction.OpCode = OpCodes.Ldarg_S;
                        instruction.Operand = method.Parameters[3];
                        break;
                    case { OpCode.Code: Code.Ldftn, Operand: MethodReference reference }:
                        reference.Parameters.Insert(0, P_pCCallbackBase);
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}