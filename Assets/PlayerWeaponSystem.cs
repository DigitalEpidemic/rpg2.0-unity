using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerWeaponSystem : MonoBehaviour {

    [SerializeField] float baseDamage = 10f;
    [SerializeField] WeaponConfig currentWeaponConfig;

    Animator animator;
    PlayerControl character;

    GameObject target;
    GameObject weaponObject;
    float lastHitTime;
    bool lookAtEnemy;

    const string ATTACK_TRIGGER = "Attack";
    const string DEFAULT_ATTACK = "DEFAULT ATTACK";
    const string PLAYER_NAME = "Player";

    void Start () {
        animator = GetComponent<Animator>();
        character = GetComponent<PlayerControl>();

        PutWeaponInHand(currentWeaponConfig);
        SetAttackAnimation();
    }

    void Update() {
        bool targetIsDead;
        bool targetIsOutOfRange;

        if (target == null) {
            targetIsDead = false;
            targetIsOutOfRange = false;
        } else {
            var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
            targetIsDead = targetHealth <= Mathf.Epsilon;

            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();

            if (lookAtEnemy && !targetIsOutOfRange && !targetIsDead) {
                Vector3 targetPosition = new Vector3(target.transform.position.x,
                                   transform.position.y,
                                   target.transform.position.z);
                transform.LookAt(targetPosition);
            }
        }

        float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
        bool characterIsDead = characterHealth <= Mathf.Epsilon;

        if (characterIsDead || targetIsOutOfRange || targetIsDead) {
            StopAllCoroutines();
        }
    }

    public void PutWeaponInHand(WeaponConfig weaponToUse) {
        currentWeaponConfig = weaponToUse;
        var weaponPrefab = weaponToUse.GetWeaponPrefab();
        GameObject dominantHand = RequestDominantHand();
        Destroy(weaponObject); // Empty hands
        weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
        weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
        weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
    }

    public void AttackTarget(GameObject targetToAttack) {
        if (gameObject.name == PLAYER_NAME && gameObject.GetComponent<PlayerControl>() != null) {
            PlayerControl playerControl = GetComponent<PlayerControl>();
            playerControl.isAttacking = true;
        }

        target = targetToAttack;
        StartCoroutine(AttackTargetRepeatedly());
    }

    public void StopAttacking() {
        if (gameObject.name == PLAYER_NAME && gameObject.GetComponent<PlayerControl>() != null) {
            PlayerControl playerControl = GetComponent<PlayerControl>();
            playerControl.isAttacking = false;
        }
        lookAtEnemy = false;
        animator.StopPlayback();
        StopAllCoroutines();
    }

    IEnumerator AttackTargetRepeatedly() {
        bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
        bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

        while (attackerStillAlive && targetStillAlive) {
            //float weaponHitRate = currentWeaponConfig.GetTimeBetweenAnimationCycles();
            //float timeToWait = weaponHitRate * character.GetAnimSpeedMultiplier();

            var animationClip = currentWeaponConfig.GetAttackAnimClip();
            float animationClipTime = animationClip.length;
            float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();

            bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

            if (isTimeToHitAgain) {
                AttackTargetOnce();
                lastHitTime = Time.time;
            }

            yield return new WaitForSeconds(timeToWait);
        }
    }

    void AttackTargetOnce() {
        lookAtEnemy = true;
        animator.SetTrigger(ATTACK_TRIGGER);
        float damageDelay = currentWeaponConfig.GetDamageDelay();
        SetAttackAnimation();
        StartCoroutine(DamageAfterDelay(damageDelay));
    }

    IEnumerator DamageAfterDelay(float delay) {
        yield return new WaitForSecondsRealtime(delay);
        target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
    }

    public WeaponConfig GetCurrentWeapon() {
        return currentWeaponConfig;
    }

    void SetAttackAnimation() {
        if (!character.GetOverrideController()) {
            Debug.Break();
            Debug.LogAssertion("Please provide: " + gameObject + " with an Animator Override Controller!");
        }

        var animatorOverrideController = character.GetOverrideController();

        animator.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
    }

    GameObject RequestDominantHand() {
        var dominantHands = GetComponentsInChildren<DominantHand>();
        int numberOfDominantHands = dominantHands.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand script found on " + gameObject.name + ", please add one!");
        Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts found on " + gameObject.name + ", please remove one!");
        return dominantHands[0].gameObject;
    }

    float CalculateDamage() {
        return baseDamage + currentWeaponConfig.GetAdditionalDamage();
    }

} // PlayerWeaponSystem
