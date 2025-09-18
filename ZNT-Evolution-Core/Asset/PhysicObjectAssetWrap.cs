using HarmonyLib;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    internal class PhysicObjectAssetWrap : PhysicObjectAsset
    {
        public bool CarryParent
        {
            get => Traverse.Create(this).Field<bool>("carryParent").Value;
            set => Traverse.Create(this).Field<bool>("carryParent").Value = value;
        }

        public float IgnoreCollisionDuration
        {
            get => Traverse.Create(this).Field<float>("ignoreCollisionDuration").Value;
            set => Traverse.Create(this).Field<float>("ignoreCollisionDuration").Value = value;
        }

        public Vector2 StartDirection
        {
            get => Traverse.Create(this).Field<Vector2>("startDirection").Value;
            set => Traverse.Create(this).Field<Vector2>("startDirection").Value = value;
        }

        public float StartForce
        {
            get => Traverse.Create(this).Field<float>("startForce").Value;
            set => Traverse.Create(this).Field<float>("startForce").Value = value;
        }

        public bool AllowRotation
        {
            get => Traverse.Create(this).Field<bool>("allowRotation").Value;
            set => Traverse.Create(this).Field<bool>("allowRotation").Value = value;
        }

        public float Friction
        {
            get => Traverse.Create(this).Field<float>("Friction").Value;
            set => Traverse.Create(this).Field<float>("Friction").Value = value;
        }

        public float Bounciness
        {
            get => Traverse.Create(this).Field<float>("Bounciness").Value;
            set => Traverse.Create(this).Field<float>("Bounciness").Value = value;
        }

        public float GravityScale
        {
            get => Traverse.Create(this).Field<float>("GravityScale").Value;
            set => Traverse.Create(this).Field<float>("GravityScale").Value = value;
        }

        public bool PlayAnimation
        {
            get => Traverse.Create(this).Field<bool>("playAnimation").Value;
            set => Traverse.Create(this).Field<bool>("playAnimation").Value = value;
        }

        public tk2dSpriteAnimation Animation
        {
            get => Traverse.Create(this).Field<tk2dSpriteAnimation>("library").Value;
            set => Traverse.Create(this).Field<tk2dSpriteAnimation>("library").Value = value;
        }
    }
}