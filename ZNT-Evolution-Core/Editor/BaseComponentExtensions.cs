using System.Reflection;
using JetBrains.Annotations;

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
}