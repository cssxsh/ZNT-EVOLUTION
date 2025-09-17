namespace ZNT.Evolution.Core.Editor
{
    public abstract class EditorComponent : BaseComponent
    {
        protected override void OnCreate()
        {
            var attribute = this.GetAttribute<SerializeInEditorAttribute>();
            EditorVisibility = attribute.VisibleInEditor;
        }
    }
}