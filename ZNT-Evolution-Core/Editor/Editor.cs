using System.Reflection;

namespace ZNT.Evolution.Core.Editor;

public abstract class Editor : BaseComponent
{
    protected Editor()
    {
        var attribute = GetType().GetCustomAttribute<SerializeInEditorAttribute>();
        if (attribute is null) return;
        EditorVisibility = new Visibility(attribute.VisibleInEditor)
        {
            CustomName = attribute.Name
        };
    }
}