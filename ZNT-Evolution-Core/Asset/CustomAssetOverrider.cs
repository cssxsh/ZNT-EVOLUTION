using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ExplodeSurface = PhysicObjectBehaviour.ExplodeSurface;

namespace ZNT.Evolution.Core.Asset;

public class CustomAssetOverrider
{
    public string Id;

    public CustomAsset Asset;

    public FieldInfo Field;

    public string Index;

    public string Action;

    public JToken Token;

    private object _original;

    private bool _saved;

    public void Submit()
    {
        lock (Asset)
        {
            if (_saved) throw new System.Exception(Id);
            if (Field.IsDefined(typeof(LayerAttribute)))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                using var json = new JTokenReader(Token);
                var input = LayerConverter.Instance.
                    ReadJson(json, Field.FieldType, null, CustomAssetUtility.Serializer);
                Field.SetValue(Asset, input);
            }
            else if (Field.FieldType == typeof(int))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (int)_original;
                var input = Token.Value<double>();
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => (int)input,
                    "+" => (int)(value + input),
                    "-" => (int)(value - input),
                    "*" => (int)(value * input),
                    "/" => (int)(value / input),
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(float))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (float)_original;
                var input = Token.Value<float>();
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    "*" => value * input,
                    "/" => value / input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(LayerMask))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (int)(LayerMask)_original;
                var input = (int)CustomAssetUtility.DeserializeObject<LayerMask>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => (LayerMask)input,
                    "+" => (LayerMask)(value | input),
                    "-" => (LayerMask)(value & ~input),
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(Color))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (Color)_original;
                var input = CustomAssetUtility.DeserializeObject<Color>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    "*" => value * input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(Vector2))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (Vector2)_original;
                var input = CustomAssetUtility.DeserializeObject<Vector2>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    "*" => value * input,
                    "/" => value / input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(Vector3))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (Vector3)_original;
                var input = CustomAssetUtility.DeserializeObject<Vector3>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(DamageMultiplierDictionary) && Index is not (null or ""))
            {
                var dictionary = (DamageMultiplierDictionary)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<DamageType>(Index);
                _original = dictionary.TryGetValue(key, out var v) ? v : 1;
                _saved = true;
                var value = (float)_original;
                var input = Token.Value<float>();
                dictionary[key] = Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    "*" => value * input,
                    "/" => value / input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                };
            }
            else if (Field.FieldType == typeof(StringGameObjectDictionary) && Index is not (null or ""))
            {
                var dictionary = (StringGameObjectDictionary)Field.GetValue(Asset);
                _original = dictionary.TryGetValue(Index, out var v) ? v : null;
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<GameObject>(Token);
                dictionary[Index] = input;
            }
            else if (Field.FieldType == typeof(ForceMultipliers) && Index is not (null or ""))
            {
                var dictionary = (ForceMultipliers)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<Layer>(Index);
                _original = dictionary.TryGetValue(key, out var v) ? v : 1;
                _saved = true;
                var value = (float)_original;
                var input = Token.Value<float>();
                dictionary[key] = Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    "*" => value * input,
                    "/" => value / input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                };
            }
            else if (Field.FieldType == typeof(ExplosionAsset[]) && Index is not (null or ""))
            {
                var array = (ExplosionAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                if (index >= array.Length) System.Array.Resize(ref array, index + 1);
                Field.SetValue(Asset, array);
                _original = array[index];
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<ExplosionAsset>(Token);
                array[index] = input;
            }
            else if (Field.FieldType == typeof(PhysicObjectAsset[]) && Index is not (null or ""))
            {
                var array = (PhysicObjectAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                if (index >= array.Length) System.Array.Resize(ref array, index + 1);
                Field.SetValue(Asset, array);
                _original = array[index];
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(Token);
                array[index] = input;
            }
            else if (Field.FieldType == typeof(Tag))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (Tag)_original;
                var input = CustomAssetUtility.DeserializeObject<Tag>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value | input,
                    "-" => value & ~input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(ExplodeSurface))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (ExplodeSurface)_original;
                var input = CustomAssetUtility.DeserializeObject<ExplodeSurface>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value | input,
                    "-" => value & ~input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.Name is nameof(HumanAsset.RageDamageType) && Field.DeclaringType == typeof(HumanAsset))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (int)DamageFlagsConverter.GetDamageFlags((DamageType)_original);
                var input = (int)CustomAssetUtility.DeserializeObject<DamageFlagsConverter.Flags>(Token);
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => (DamageType)(int.MinValue | input),
                    "+" => (DamageType)(int.MinValue | value | input),
                    "-" => (DamageType)(int.MinValue | (value & ~input)),
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                using var json = new JTokenReader(Token);
                var input = CustomAssetUtility.Serializer.Deserialize(json, Field.FieldType);
                Field.SetValue(Asset, input);
            }
        }
    }

    public void Reset()
    {
        lock (Asset)
        {
            if (!_saved) return;
            if (Field.FieldType == typeof(DamageMultiplierDictionary))
            {
                var dictionary = (DamageMultiplierDictionary)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<DamageType>(Index);
                if ((float)_original is 1) dictionary.Remove(key);
                else dictionary[key] = (float)_original;
            }
            else if (Field.FieldType == typeof(StringGameObjectDictionary))
            {
                var dictionary = (StringGameObjectDictionary)Field.GetValue(Asset);
                if ((GameObject)_original is null) dictionary.Remove(Index);
                else dictionary[Index] = (GameObject)_original;
            }
            else if (Field.FieldType == typeof(ForceMultipliers))
            {
                var dictionary = (ForceMultipliers)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<Layer>(Index);
                if ((float)_original is 1) dictionary.Remove(key);
                else dictionary[key] = (float)_original;
            }
            else if (Field.FieldType == typeof(ExplosionAsset[]))
            {
                var array = (ExplosionAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                array[index] = (ExplosionAsset)_original;
            }
            else if (Field.FieldType == typeof(PhysicObjectAsset[]))
            {
                var array = (PhysicObjectAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                array[index] = (PhysicObjectAsset)_original;
            }
            else
            {
                Field.SetValue(Asset, _original);
            }

            _saved = false;
        }
    }
}