using System;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using ZNT.LevelEditor;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Movement")]
[DisallowMultipleComponent]
public class MovingObjectEditor : Editor, IEditorOverride
{
    private PropMoveable Moveable => field ??= GetComponent<PropMoveable>();

    private Tweener Tween;

    public float CurrentSpeed
    {
        get => Moveable.CurrentSpeed;
        set => Moveable.SetCurrentSpeed(value);
    }

    [SerializeInEditor(name: "Speed Ease")]
    public Ease SpeedEase = Ease.InOutQuad;

    [SerializeInEditor(name: "Speed Ease Duration")]
    public float Duration;

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        var panel = menu.MainContainer;
        var container = panel.parent;
        var target = menu.SerializeGameObject;
        var moveable = target.Components.Find(t => t.Data is PropMoveable);
        if (moveable is null) return false;
        var prev = (RectTransform)container.Find($"{moveable.Name} Panel");
        menu.MainContainer = prev;
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
            menu.MainContainer = panel;
        }
    }

    protected override void OnCreate()
    {
        Moveable.Event<UnityEvent>("OnMove").AddListener(OnMove);
        Moveable.Event<UnityEvent>("OnStop").AddListener(OnStop);
    }

    private void OnMove()
    {
        if (SpeedEase is Ease.Unset || Duration <= 0) return;
        SpeedTween(0, Moveable.Speed);
    }

    private void OnStop()
    {
        if (Moveable.StopAtNextStep) return;
        if (SpeedEase is Ease.Unset || Duration <= 0) return;
        SpeedTween(Moveable.Speed, 0);
    }

    public Tweener SpeedTween(float start, float end)
    {
        Tween?.Kill();
        CurrentSpeed = start;
        return Tween = DOTween
            .To(value => CurrentSpeed = value, start, end, Duration)
            .SetEase(SpeedEase);
    }

    private void OnDespawned()
    {
        Tween?.Kill();
        SpeedEase = Ease.Unset;
        Duration = 0;
    }
}