using System.Linq;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public class SpawnPointAsset : CustomAssetObject
    {
        public CustomAssetObject[] spawnableObjects;
 
        public float interval = 2f;
 
        public float startDelay;
 
        public int count;
 
        public bool infinite;
 
        public bool active = true;
 
        public bool moveOnStart = true;
 
        public Vector3 orientation = Vector3.forward;
 
        public Vector2 direction = Vector3.right;
 
        public float speed;
 
        public float duration;
 
        public bool applyDamages = true;
        
        public CharacterMutation spawnMutation;
        
        public MoveSpeed defaultSpeed;
        
        public override void LoadFromAsset(GameObject gameObject)
        {
            base.LoadFromAsset(gameObject);
            var spawn = gameObject.GetComponent<SpawnPoint>();
            spawn.SpawnableObjects = spawnableObjects.ToList();
            spawn.Interval = interval;
            spawn.StartDelay = startDelay;
            spawn.Count = count;
            spawn.Infinite = infinite;
            spawn.Active = active;
            spawn.MoveOnStart = moveOnStart;
            spawn.Orientation = orientation;
            spawn.Direction = direction;
            spawn.Speed = speed;
            spawn.Duration = duration;
            spawn.ApplyDamages = applyDamages;
            if (!(spawn is CharacterSpawnPoint character)) return;
            character.SpawnMutation = spawnMutation;
            character.DefaultSpeed = defaultSpeed;
        }
    }
}