using System.Linq;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZNT.LevelEditor;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Sprite", devOnly: true)]
[DisallowMultipleComponent]
public class SpriteEditor : Editor, IEditorOverride, IEditorUpdate
{
    [JsonIgnore]
    private tk2dSpriteAnimator Animator => field ??= GetComponentInChildren<tk2dSpriteAnimator>();

    [JsonIgnore]
    private tk2dBaseSprite Sprite => field ??= Animator?.Sprite ?? GetComponentInChildren<tk2dBaseSprite>();

    [JsonIgnore]
    [SerializeInEditor(name: "Sprite Animation Clip")]
    public string SpriteAnimationClip
    {
        get => Animator?.CurrentClip?.name;
        set
        {
            if (Animator is null) return;
            if (Animator.CurrentClip?.name == value) return;
            Animator.Play(value);
        }
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Sprite Definition")]
    public string SpriteDefinition
    {
        get => Sprite.CurrentSprite.name;
        set => Sprite.spriteId = Sprite.GetSpriteIdByName(value);
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Sprite Color")]
    public Color SpriteColor
    {
        get => Sprite.color;
        set => Sprite.color = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Sprite Layer")]
    public string SpriteLayer
    {
        get => Sprite.CachedRenderer.sortingLayerName;
        set => Sprite.CachedRenderer.sortingLayerName = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Sprite Order")]
    public int SpriteOrder
    {
        get => Sprite.SortingOrder;
        set => Sprite.SortingOrder = value;
    }

    [JsonIgnore]
    [SerializeInEditor(name: "Edit Sprite Collection")]
    [LevelEditorButton(nameof(EditSpriteCollection))]
    public bool Editing { private set; get; }

    public bool OverrideMemberUi(SelectionMenu menu, EditorComponent component, MemberInfo member)
    {
        switch (member.Name)
        {
            case nameof(SpriteAnimationClip):
            {
                if (Animator is null) return true;
                Traverse.Create(Animator).Field<tk2dSpriteAnimationClip>("currentClip").Value ??= (
                    from clip in Animator.Library.clips
                    where !clip.Empty && clip.frames.Any(frame => frame.spriteId == Sprite.spriteId)
                    select clip
                ).FirstOrDefault();
                var binder = menu.ListBinder();
                var names =
                    from clip in Animator.Library.clips
                    where !clip.Empty
                    select clip.name;
                binder.BindStringListField(component, member, names.ToArray());
            }
                return true;
            case nameof(SpriteDefinition):
            {
                var binder = menu.ListBinder();
                var names = Animator is null
                    ? from definition in Sprite.Collection.spriteDefinitions
                    where definition.Valid
                    select definition.name
                    : from frame in Animator.CurrentClip.frames
                    let definition = frame.spriteCollection.spriteDefinitions[frame.spriteId]
                    select definition.name;
                binder.BindStringListField(component, member, names.ToArray());
                _dropdown = (Dropdown)Traverse.Create(binder).Field<UIBehaviour[]>("uiComponents").Value[0];
            }
                return true;
            case nameof(SpriteLayer):
            {
                var binder = menu.ListBinder();
                var names =
                    from layer in SortingLayer.layers
                    select layer.name;
                binder.BindStringListField(component, member, names.ToArray());
            }
                return true;
        }

        return false;
    }

    public void OnEditorOpen()
    {
        Animator?.AnimationPlayed += OnAnimationChanged;
    }

    public void OnEditorUpdate()
    {
        // ...
    }

    public void OnEditorClose()
    {
        Animator?.AnimationPlayed -= OnAnimationChanged;
    }

    private void EditSpriteCollection()
    {
        var material = Sprite.Collection.materials[0];
        var texture = Sprite.Collection.textures[0];
        var path = $"{texture.name}.png";
        if (Editing)
        {
            if (!System.IO.File.Exists(path)) return;
            var readable = Asset.SpriteExtractor.MarkReadable(texture);
            readable.LoadImage(System.IO.File.ReadAllBytes(path));
            material.mainTexture = readable;
            Editing = false;
        }
        else
        {
            Editing = true;
            if (System.IO.File.Exists(path)) return;
            var readable = Asset.SpriteExtractor.MarkReadable(texture);
            System.IO.File.WriteAllBytes(path, readable.EncodeToPNG());
        }
    }

    [JsonIgnore]
    private Dropdown _dropdown;

    private void OnAnimationChanged(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
        if (_dropdown is null) return;
        _dropdown.options.Clear();
        _dropdown.options.AddRange(
            from frame in clip.frames
            let definition = frame.spriteCollection.spriteDefinitions[frame.spriteId]
            select new Dropdown.OptionData(definition.name));
        _dropdown.SetValueWithoutNotify(0);
        _dropdown.RefreshShownValue();
    }
}