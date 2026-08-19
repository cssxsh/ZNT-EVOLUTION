using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor;

public static class BaseComponentExtensions
{
    [UsedImplicitly]
    public static void SetVisible(this BaseComponent component, bool value)
    {
        typeof(Visibility).GetTypeInfo()
            .GetDeclaredField("visible")
            .SetValueDirect(__makeref(component.EditorVisibility), value);
    }

    [UsedImplicitly]
    public static void SetDevOnly(this BaseComponent component, bool value)
    {
        typeof(Visibility).GetTypeInfo()
            .GetDeclaredField("devOnly")
            .SetValueDirect(__makeref(component.EditorVisibility), value);
    }

    [UsedImplicitly]
    public static void SetIgnoreSerialization(this BaseComponent component, bool value)
    {
        typeof(Visibility).GetTypeInfo()
            .GetDeclaredField("ignoreSerialization")
            .SetValueDirect(__makeref(component.EditorVisibility), value);
    }

    [UsedImplicitly]
    public static T CreateDelegate<T>(this BaseComponent component, string method) where T : System.Delegate
    {
        return System.Delegate.CreateDelegate(typeof(T), component, method) as T;
    }

    [UsedImplicitly]
    public static T Event<T>(this BaseComponent component, string name) where T : UnityEngine.Events.UnityEventBase
    {
        return Traverse.Create(component).Field("events").Field<T>(name).Value;
    }

    [UsedImplicitly]
    public static void SetSerialize(this SerialIdentifier identifier, bool value)
    {
        Traverse.Create(identifier).Field<bool>("serialize").Value = value;
    }

    [UsedImplicitly]
    public static T GetEffect<T>(this Trigger trigger) where T : TriggerEffect
    {
        var effect = trigger.gameObject.GetComponentSafe<T>();
        // ReSharper disable once InvertIf
        if (Traverse.Create(trigger).Field<TriggerEffect[]>("effects").Value is { } effects &&
            !effects.Contains(effect))
        {
            Traverse.Create(trigger).Field<TriggerEffect[]>("effects").Value = null;
            _ = trigger.Effects;
            // Traverse.Create(trigger).Field<INoAllocEffect[]>("noAllocEffects").Value
        }

        return effect;
    }

    [UsedImplicitly]
    public static void ShowDirection(this SpawnPoint spawn, bool value)
    {
        Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDirection").Value = true;
    }

    [UsedImplicitly]
    public static void ShowSpeed(this SpawnPoint spawn, bool value)
    {
        Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowSpeed").Value = true;
    }

    [UsedImplicitly]
    public static void ShowDuration(this SpawnPoint spawn, bool value)
    {
        Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDuration").Value = true;
    }

    [UsedImplicitly]
    public static void ShowMoveOnStart(this SpawnPoint spawn, bool value)
    {
        Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowMoveOnStart").Value = true;
    }

    [UsedImplicitly]
    public static void ShowDamages(this SpawnPoint spawn, bool value)
    {
        Traverse.Create(spawn).Field("levelEditorOptions").Field<bool>("ShowDamages").Value = true;
    }

    [UsedImplicitly]
    public static CorpseType GetSpawnType(this CharacterSpawnPoint spawn)
    {
        return (CorpseType)Traverse.Create(spawn).Field<System.Enum>("spawnType").Value;
    }

    [UsedImplicitly]
    public static void SetMagazine(this Weapon weapon, int size)
    {
        weapon.DefaultMag = new Magazine(size);
        weapon.Initialize();
    }

    [UsedImplicitly]
    public static bool IsTalking(this Patroller patroller)
    {
        return Traverse.Create(typeof(Dialogue))
            .Field<Dictionary<UnityEngine.Transform, Dialogue>>("Talking").Value
            .ContainsKey(patroller.Root.transform);
    }

    [UsedImplicitly]
    public static SupportedTypeBinder TextBinder(this SelectionMenu menu)
    {
        var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("typePrefabs").Value;
        return menu.InstantiateCustomBinder(prefabs[EditorComponent.SupportedType.String]);
    }

    [UsedImplicitly]
    public static SupportedTypeBinder ListBinder(this SelectionMenu menu)
    {
        return menu.InstantiateCustomBinder(menu.CustomBinders.IntStringList);
    }

    [UsedImplicitly]
    public static SupportedTypeBinder DirectionBinder(this SelectionMenu menu)
    {
        var prefabs = Traverse.Create(menu).Field<SupportedTypePrefabs>("customDrawerPrefabs").Value;
        return menu.InstantiateCustomBinder(prefabs[EditorComponent.SupportedType.Vector3]);
    }
}