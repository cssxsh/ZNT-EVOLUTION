using System;
using System.Reflection;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Prop Movements")]
[DisallowMultipleComponent]
public class PropMoveableEditor : Editor, IEditorOverride
{
    [SerializeInEditor(name: "Speed Ease")]
    public Ease SpeedEase = Ease.InOutQuad;

    [SerializeInEditor(name: "Speed Ease Duration")]
    public float Duration;

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        var panel = Traverse.Create(menu).Field<RectTransform>("mainContainer").Value;
        var container = panel.parent;
        var target = Traverse.Create(menu).Field<EditorGameObject>("serializeGameObject").Value;
        var moveable = target.Components.Find(t => t.Data is PropMoveable);
        if (moveable is null) return false;
        var prev = (RectTransform)container.Find($"{moveable.Name} Panel");
        Traverse.Create(menu).Field<RectTransform>("mainContainer").Value = prev;
        try
        {
            menu.SetDefaultUi(component, member);
            var index = member.Name switch
            {
                nameof(SpeedEase) => 2,
                nameof(Duration) => 3,
                _ => throw new IndexOutOfRangeException(member.Name)
            };
            prev.GetChild(prev.childCount - 1).SetSiblingIndex(index);
            return true;
        }
        finally
        {
            Traverse.Create(menu).Field<RectTransform>("mainContainer").Value = panel;
        }
    }

    [NonSerialized]
    private Tweener Tween;

    public Tweener SpeedTween(PropMoveable moveable, float start, float end)
    {
        var speed = Traverse.Create(moveable).Field<float>("currentSpeed");
        Tween?.Kill();
        return Tween = DOTween
            .To(value => speed.Value = value, start, end, Duration)
            .SetEase(SpeedEase);
    }
}