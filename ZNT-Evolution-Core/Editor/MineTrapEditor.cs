using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Explosion")]
    [DisallowMultipleComponent]
    public class MineTrapEditor : Editor, IEditorOverride
    {
        private Trigger Trigger => GetComponentInChildren<Trigger>();

        [SerializeInEditor(name: "Detected Human")]
        public bool DetectedHuman
        {
            get => Trigger.WithTags.HasFlag(Tag.Human);
            set => Trigger.WithTags = value ? Trigger.WithTags.Add(Tag.Human) : Trigger.WithTags.Remove(Tag.Human);
        }

        [SerializeInEditor(name: "Detected Zombie")]
        public bool DetectedZombie
        {
            get => Trigger.WithTags.HasFlag(Tag.Zombie);
            set => Trigger.WithTags = value ? Trigger.WithTags.Add(Tag.Zombie) : Trigger.WithTags.Remove(Tag.Zombie);
        }

        private MineBehaviour Behaviour => GetComponentInChildren<MineBehaviour>();

        private ExplosionAsset Explosion
        {
            get => Traverse.Create(Behaviour).Field<ExplosionAsset>("explosion").Value;
            set => Traverse.Create(Behaviour).Field<ExplosionAsset>("explosion").Value = value;
        }

        private Dictionary<string, ExplosionAsset> _selectable;

        [SerializeInEditor(name: "Explosion")]
        public string Selected
        {
            get => Explosion.HierarchyName;
            set => Explosion = _selectable[value];
        }

        protected override void OnCreate()
        {
            _selectable = FindObjectsOfType<ExplosionAsset>().ToDictionary(explosion => explosion.HierarchyName);
        }

        public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
        {
            if (member.Name != nameof(Selected)) return false;
            CustomBinder(menu).BindStringListField(component, member, _selectable.Keys.ToList());
            return true;
        }
    }
}