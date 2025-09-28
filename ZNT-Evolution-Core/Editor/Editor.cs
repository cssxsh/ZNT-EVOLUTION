using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor
{
    public abstract class Editor : BaseComponent
    {
        protected Editor()
        {
            EditorVisibility = true;
        }

        protected static SupportedTypeBinder CustomBinder(SelectionMenu menu)
        {
            return menu.InstantiateCustomBinder(menu.CustomBinders.IntStringList);
        }
    }
}