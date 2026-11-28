using UnityEngine;

namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Chemist")]
[DisallowMultipleComponent]
public class ChemistEditor : Editor, IActivable
{
    [SerializeInEditor(name: "Is Active")]
    public bool IsActive { get; set; }

    public void SetActive(bool state) => IsActive = state;

    public void SetActive() => SetActive(true);

    public void SetInactive() => SetActive(false);

    public void ToggleActivation() => SetActive(!IsActive);

    public void OnDamage(Parameters parameters)
    {
        if (!IsActive) return;
        var attacker = parameters.GetValue<Transform>("Attacker");
        if (attacker.GetComponent<Health>() is not CharacterHealth { Character: {} zombie }) return;
        if (zombie.CharacterType is not CharacterType.Zombie) return;
        if (zombie.GetComponent<ZombieEditor>() is not {} editor) return;
        editor.ApplyEnemy();
    }
}