using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Weapon"))]
public class WeaponConfig : ScriptableObject {

    public Transform gripTransform;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] float timeBetweenAnimationCycles = 0.5f;
    [SerializeField] float maxAttackRange = 2f;
    [SerializeField] float additionalDamage = 10f;
    [SerializeField] float damageDelay = 0.5f;

    public float GetTimeBetweenAnimationCycles() {
        //return attackAnimation.length;
        return timeBetweenAnimationCycles;
    }

    public float GetMaxAttackRange() {
        return maxAttackRange;
    }

    public GameObject GetWeaponPrefab() {
        return weaponPrefab;
    }

    public AnimationClip GetAttackAnimClip() {
        RemoveAnimationEvents();
        return attackAnimation;
    }

    public float GetAdditionalDamage() {
        return additionalDamage;
    }

    public float GetDamageDelay() {
        return damageDelay;
    }

    // Removes bugs/crashes from asset packs
    private void RemoveAnimationEvents() {
        if (attackAnimation) {
            attackAnimation.events = new AnimationEvent[0];
        }
    }

} // WeaponConfig
