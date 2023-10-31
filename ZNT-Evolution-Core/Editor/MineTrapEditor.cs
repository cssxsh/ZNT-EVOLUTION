using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Explosion")]
    [DisallowMultipleComponent]
    public class MineTrapEditor : EditorComponent
    {
        private MineBehaviour Behaviour => GetComponentInChildren<MineBehaviour>();

        private ExplosionAsset _explosion;

        private ExplosionAsset Explosion()
        {
            if (_explosion != null) return _explosion;
            var field = Traverse.Create(Behaviour).Field<ExplosionAsset>(name: "explosion");
            var impl = field.Value;
            _explosion = Instantiate(impl);
            field.Value = _explosion;
            return _explosion;
        }

        [SerializeInEditor(name: "Damage")]
        public float Damage
        {
            get => Explosion().Damage;
            set => Explosion().Damage = value;
        }

        [SerializeInEditor(name: "Damage Radius")]
        public float DamageRadius
        {
            get => Explosion().DamageRadius;
            set => Explosion().DamageRadius = value;
        }

        [SerializeInEditor(name: "Apply Breakable")]
        public bool Breakable
        {
            get => Explosion().ApplyDamageOn.HasFlag(Tag.Breakable);
            set => Explosion().ApplyDamageOn =
                value ? Explosion().ApplyDamageOn.Add(Tag.Breakable) : Explosion().ApplyDamageOn.Remove(Tag.Breakable);
        }

        [SerializeInEditor(name: "Apply Human")]
        public bool Human
        {
            get => Explosion().ApplyDamageOn.HasFlag(Tag.Human);
            set => Explosion().ApplyDamageOn =
                value ? Explosion().ApplyDamageOn.Add(Tag.Human) : Explosion().ApplyDamageOn.Remove(Tag.Human);
        }

        [SerializeInEditor(name: "Apply Zombie")]
        public bool Zombie
        {
            get => Explosion().ApplyDamageOn.HasFlag(Tag.Zombie);
            set => Explosion().ApplyDamageOn =
                value ? Explosion().ApplyDamageOn.Add(Tag.Zombie) : Explosion().ApplyDamageOn.Remove(Tag.Zombie);
        }
    }
}