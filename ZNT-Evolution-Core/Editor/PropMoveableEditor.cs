using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Prop Movements")]
[DisallowMultipleComponent]
public class PropMoveableEditor : Editor
{
    [SerializeInEditor(name: "Speed Ease")]
    public Ease SpeedEase = Ease.InOutQuad;

    [SerializeInEditor(name: "Speed Ease Duration")]
    public float Duration;

    internal TweenerCore<float, float, FloatOptions> Tweener;
}