using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor
{
    [SerializeInEditor(name: "Explosion")]
    [DisallowMultipleComponent]
    public class MineTrapEditor : EditorComponent
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

        public Tag ApplyDamageOn
        {
            get => Explosion().ApplyDamageOn;
            set => Explosion().ApplyDamageOn = value;
        }

        [SerializeInEditor(name: "Damage Breakable")]
        public bool DamageBreakable
        {
            get => ApplyDamageOn.HasFlag(Tag.Breakable);
            set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Breakable) : ApplyDamageOn.Remove(Tag.Breakable);
        }

        [SerializeInEditor(name: "Damage Human")]
        public bool DamageHuman
        {
            get => ApplyDamageOn.HasFlag(Tag.Human);
            set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Human) : ApplyDamageOn.Remove(Tag.Human);
        }

        [SerializeInEditor(name: "Damage Zombie")]
        public bool DamageZombie
        {
            get => ApplyDamageOn.HasFlag(Tag.Zombie);
            set => ApplyDamageOn = value ? ApplyDamageOn.Add(Tag.Zombie) : ApplyDamageOn.Remove(Tag.Zombie);
        }
        
        [SerializeInEditor(name: "Force")]
        public float Force
        {
            get => Explosion().Force;
            set => Explosion().Force = value;
        }

        public Tag ApplyForceOn
        {
            get => Explosion().ApplyForceOn;
            set => Explosion().ApplyForceOn = value;
        }

        [SerializeInEditor(name: "Force Human")]
        public bool ForceHuman
        {
            get => ApplyForceOn.HasFlag(Tag.Human);
            set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Human) : ApplyForceOn.Remove(Tag.Human);
        }

        [SerializeInEditor(name: "Force Zombie")]
        public bool ForceZombie
        {
            get => ApplyForceOn.HasFlag(Tag.Zombie);
            set => ApplyForceOn = value ? ApplyForceOn.Add(Tag.Zombie) : ApplyForceOn.Remove(Tag.Zombie);
        }
    }
}