using System.Reflection;
using System.Text.RegularExpressions;
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

    public string Value;

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
                var input = LayerConverter.TextToLayer(Value);
                Field.SetValue(Asset, _original switch
                {
                    LayerMask => (LayerMask)input,
                    _ => input
                });
            }
            else if (Field.FieldType == typeof(string))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                Field.SetValue(Asset, Value);
            }
            else if (Field.FieldType == typeof(bool))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                Field.SetValue(Asset, bool.Parse(Value));
            }
            else if (Field.FieldType == typeof(int))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (int)_original;
                var input = double.Parse(Value);
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
                var input = float.Parse(Value);
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
                var input = (int)CustomAssetUtility.DeserializeObject<LayerMask>(Value);
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
                ColorUtility.TryParseHtmlString(Value, out var input);
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
                var match = Vector2Regex.Match(Value);
                if (!match.Success) throw new System.FormatException($"{Id} - {Value}");
                var input = new Vector2(
                    x: float.Parse(match.Groups[1].Value),
                    y: float.Parse(match.Groups[2].Value));
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
                var match = Vector3Regex.Match(Value);
                if (!match.Success) throw new System.FormatException($"{Id} - {Value}");
                var input = new Vector3(
                    x: float.Parse(match.Groups[1].Value),
                    y: float.Parse(match.Groups[2].Value),
                    z: float.Parse(match.Groups[3].Value));
                Field.SetValue(Asset, Action switch
                {
                    null or "" or "=" => input,
                    "+" => value + input,
                    "-" => value - input,
                    _ => throw new System.ArgumentException($"{Id} - {Action}")
                });
            }
            else if (Field.FieldType == typeof(AnimationCurve))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var input = AnimationCurveConverter.TextToAnimationCurve(Value);
                Field.SetValue(Asset, input);
            }
            else if (Field.FieldType == typeof(Range))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var match = Vector2Regex.Match(Value);
                if (!match.Success) throw new System.FormatException($"{Id} - {Value}");
                var input = new Range(
                    minValue: float.Parse(match.Groups[1].Value),
                    maxValue: float.Parse(match.Groups[2].Value));
                Field.SetValue(Asset, input);
            }
            else if (Field.FieldType == typeof(DamageMultiplierDictionary))
            {
                var dictionary = (DamageMultiplierDictionary)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<DamageType>(Index);
                _original = dictionary.TryGetValue(key, out var v) ? v : 1;
                _saved = true;
                var value = (float)_original;
                var input = float.Parse(Value);
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
            else if (Field.FieldType == typeof(StringGameObjectDictionary))
            {
                var dictionary = (StringGameObjectDictionary)Field.GetValue(Asset);
                _original = dictionary.TryGetValue(Index, out var v) ? v : null;
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<GameObject>(Value);
                dictionary[Index] = input;
            }
            else if (Field.FieldType == typeof(ForceMultipliers))
            {
                var dictionary = (ForceMultipliers)Field.GetValue(Asset);
                var key = CustomAssetUtility.DeserializeObject<Layer>(Index);
                _original = dictionary.TryGetValue(key, out var v) ? v : 1;
                _saved = true;
                var value = (float)_original;
                var input = float.Parse(Value);
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
            else if (Field.FieldType == typeof(ExplosionAsset[]))
            {
                var array = (ExplosionAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                if (index >= array.Length) System.Array.Resize(ref array, index + 1);
                Field.SetValue(Asset, array);
                _original = array[index];
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<ExplosionAsset>(Value);
                array[index] = input;
            }
            else if (Field.FieldType == typeof(PhysicObjectAsset[]))
            {
                var array = (PhysicObjectAsset[])Field.GetValue(Asset);
                var index = int.Parse(Index);
                if (index >= array.Length) System.Array.Resize(ref array, index + 1);
                Field.SetValue(Asset, array);
                _original = array[index];
                _saved = true;
                var input = CustomAssetUtility.DeserializeObject<PhysicObjectAsset>(Value);
                array[index] = input;
            }
            else if (Field.FieldType == typeof(Tag))
            {
                _original = Field.GetValue(Asset);
                _saved = true;
                var value = (Tag)_original;
                var input = CustomAssetUtility.DeserializeObject<Tag>(Value);
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
                var input = CustomAssetUtility.DeserializeObject<ExplodeSurface>(Value);
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
                var input = (int)CustomAssetUtility.DeserializeObject<DamageFlagsConverter.Flags>(Value);
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
                using var json = new JTokenReader(Value);
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

    private static readonly Regex Vector2Regex = new(
        @"^[\(\[]([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)[\)\]]$",
        RegexOptions.Compiled);

    private static readonly Regex Vector3Regex = new(
        @"^[\(\[]([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+), ([+-]?\d*\.?\d+)[\)\]]",
        RegexOptions.Compiled);
}