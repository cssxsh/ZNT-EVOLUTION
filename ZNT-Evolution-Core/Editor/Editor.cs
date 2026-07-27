using System.Reflection;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor;

public abstract class Editor : BaseComponent
{
    protected Editor()
    {
        var attribute = GetType().GetCustomAttribute<SerializeInEditorAttribute>();
        if (attribute == null) return;
        EditorVisibility = new Visibility(attribute.VisibleInEditor)
        {
            CustomName = attribute.Name
        };
    }
}