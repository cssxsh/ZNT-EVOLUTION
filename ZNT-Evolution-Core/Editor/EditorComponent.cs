using System.Collections.Generic;
using Framework.Events;
using HarmonyLib;

namespace ZNT.Evolution.Core.Editor
{
    public abstract class EditorComponent : BaseComponent
    {
        protected override void OnCreate()
        {
            var attribute = this.GetAttribute<SerializeInEditorAttribute>();
            EditorVisibility = attribute.VisibleInEditor;
            
            var cached = Traverse.Create<SignalReceiver>()
                .Field<Dictionary<string, System.Type>>(name: "cachedType").Value;
            var type = GetType();
            cached[type.Name] = type;
        }
    }
}