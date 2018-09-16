using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class PlayerControl : MonoBehaviour {

    [SerializeField] AnimatorOverrideController animatorOverrideController;

    Animator anim;
    NavMeshAgent navMeshAgent;

    CameraRaycaster cameraRaycaster;
    PlayerWeaponSystem weaponSystem;

    //GameObject updatedTarget;
    public bool isAttacking;

    const string DEATH_TRIGGER = "Death";

    bool walking;
    bool isAlive = true;
    float speed = 1.0f;

    void Awake() {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        weaponSystem = GetComponent<PlayerWeaponSystem>();
    }

    void Update() {
        if (anim.GetBool("Attack")) {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            walking = false;
        }

        if (isAlive) {
            if (Input.GetMouseButton(0)) { // TODO Change to input binding
                print(cameraRaycaster.hit.transform.gameObject.name);
                switch (cameraRaycaster.layerHit) {
                    case Layer.Enemy: // Enemy layer
                        //Debug.Log("TODO: Implement attacking enemy.");
                        Enemy();
                        break;
                    default: // Every layer that is left
                        MovePlayer();
                        break;
                }

            }
            AnimatePlayer();
        }
    }

    public AnimatorOverrideController GetOverrideController() {
        return animatorOverrideController;
    }

    void MovePlayer() {
        weaponSystem.StopAttacking();
        speed += Time.deltaTime * 2f;

        if (Input.GetMouseButtonDown(0)) {
            //navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }

        if (Vector3.Distance(transform.position, cameraRaycaster.hit.point) <= 0.8f) {
            anim.speed = 1.5f;
            if (speed >= 0.5f) {
                speed -= Time.deltaTime * 5f;
            } else {
                speed = 0.5f;
            }
        } else {
            anim.speed = 1f;
            speed += Time.deltaTime * 5f;
        }

        walking = true;
        navMeshAgent.SetDestination(cameraRaycaster.hit.point);
        navMeshAgent.isStopped = false;
    }

    bool IsTargetInRange(GameObject target) {
        //updatedTarget = target;
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    void Enemy() {
        GameObject enemy = cameraRaycaster.hit.transform.gameObject;
        if (IsTargetInRange(enemy)) {
            navMeshAgent.isStopped = true;
            walking = false;
            weaponSystem.AttackTarget(enemy);
        } else {
            StartCoroutine(MoveAndAttack(enemy));
        }
    }

    IEnumerator MoveToTarget(GameObject enemy) {
        //updatedTarget = enemy;
        navMeshAgent.isStopped = false;
        walking = true;
        navMeshAgent.SetDestination(enemy.transform.position);
        while (!IsTargetInRange(enemy)) {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(GameObject enemy) {
        //updatedTarget = enemy;
        yield return StartCoroutine(MoveToTarget(enemy));
        weaponSystem.AttackTarget(enemy);
    }

    void AnimatePlayer() {
        speed = Mathf.Clamp(speed, 0f, 1f);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon) {
                walking = false;
            }
        }

        anim.SetBool("IsWalking", walking);
        anim.SetFloat("Speed", speed);
    }

    public void Kill() {
        isAlive = false;
        anim.SetTrigger(DEATH_TRIGGER);
    }

} // PlayerControl