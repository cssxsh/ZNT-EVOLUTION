using System;
using System.IO;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using UnityEngine;
using ZNT.Evolution.Core.Asset;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core;

internal static class SoundManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SoundManager), "PlayOneShot", typeof(string), typeof(Vector3))]
    public static bool PlayOneShot(string eventName, Vector3 position)
    {
        if (eventName is null) return true;
        // ReSharper disable once InvertIf
        if (eventName.StartsWith("file://"))
        {
            FmodAssetIndex.PathIndex.TryGetValue(eventName, out var asset);
            if (asset is not SoundAsset sound) throw new FileNotFoundException(eventName);
            _ = sound.Event.createInstance(out var instance);
            _ = instance.setCallback(ProgrammerSound, EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND);
            _ = instance.setUserData(sound.Sound.handle);
            _ = instance.set3DAttributes(position.To3DAttributes());
            _ = instance.start();
            _ = instance.release();
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SoundManager), "GetEvent")]
    public static bool GetEvent(string eventId, ref EventInstance __result)
    {
        if (eventId is null) return true;
        // ReSharper disable once InvertIf
        if (eventId.StartsWith("file://"))
        {
            FmodAssetIndex.PathIndex.TryGetValue(eventId, out var asset);
            if (asset is not SoundAsset sound) throw new FileNotFoundException(eventId);
            _ = sound.Event.createInstance(out var instance);
            _ = instance.setCallback(ProgrammerSound, EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND);
            _ = instance.setUserData(sound.Sound.handle);
            __result = instance;
            return false;
        }

        return true;
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT ProgrammerSound(EVENT_CALLBACK_TYPE type, EventInstance instance, IntPtr parameters)
    {
        if (type is not EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND) return FMOD.RESULT.OK;
        var result = instance.getUserData(out var sound);
        if (result is not FMOD.RESULT.OK) return result;
        var properties = Marshal.PtrToStructure<PROGRAMMER_SOUND_PROPERTIES>(parameters);
        properties.sound = sound;
        properties.subsoundIndex = -1;
        Marshal.StructureToPtr(properties, parameters, false);
        return FMOD.RESULT.OK;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SoundEventPlayer), "PlayWithParam")]
    public static bool PlayWithParam(string param, float value)
    {
        // ReSharper disable once InvertIf
        if (param is "TalkSelect" && VoiceAsset.Elements.TryGetValue((int)value, out var asset))
        {
            SoundManager.PlayOneShot(asset.path);
            return false;
        }

        return true;
    }
}