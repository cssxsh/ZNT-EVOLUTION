using System;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ZNT.Evolution.Core.Asset
{
    public class ScriptableObjectConverter : CustomCreationConverter<ScriptableObject>
    {
        public override ScriptableObject Create(Type objectType) => ScriptableObject.CreateInstance(objectType);
    }
}