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