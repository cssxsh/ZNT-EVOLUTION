using UnityEngine;
using ZNT.Evolution.Core.Asset;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Rage")]
[DisallowMultipleComponent]
public class RageEditor : Editor
{
    public DamageDictionary<ExplosionAsset> ExplosionAssets;
}