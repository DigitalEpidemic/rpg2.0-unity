using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WeaponSystem))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Character))]
public class EnemyAI : MonoBehaviour {

    [SerializeField] float chaseRadius = 6f;
    [SerializeField] WaypointContainer patrolPath;
    [SerializeField] float waypointTolerance = 2f;
    [SerializeField] float waypointWaitTime = 2f;

    Character character;
    PlayerControl player;

    float distanceToPlayer;
    float currentWeaponRange;
    int nextWaypointIndex;

    enum State { idle, patrolling, attacking, chasing }
#pragma warning disable 0414
    State state = State.idle;
#pragma warning restore 0414

    void Start() {
        character = GetComponent<Character>();
        player = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update() {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        WeaponSystem weaponSystem = GetComponent<WeaponSystem>(); // No performance issue
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

        bool inWeaponRadius = distanceToPlayer <= currentWeaponRange;
        bool inChaseRadius = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
        bool outsideChaseRadius = distanceToPlayer > chaseRadius;

        if (inWeaponRadius) {
            print("inWeaponRadius");
            StopAllCoroutines();
            state = State.attacking;
            transform.LookAt(player.gameObject.transform);
            weaponSystem.AttackTarget(player.gameObject);
            character.GetNavMeshAgent().isStopped = true;
            character.GetNavMeshAgent().velocity = Vector3.zero;
        }

        if (outsideChaseRadius) {
            print("outsideChaseRadius");
            StopAllCoroutines();
            StartCoroutine(Patrol());
        }

        if (inChaseRadius) {
            print("inChaseRadius");
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            character.GetNavMeshAgent().isStopped = false;
            StartCoroutine(ChasePlayer());
        }
    }

    IEnumerator Patrol() {
        state = State.patrolling;

        while (patrolPath != null) {
            Vector3 nextWayPointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
            character.SetDestination(nextWayPointPos);
            CycleWaypointWhenClose(nextWayPointPos);
            yield return new WaitForSeconds(waypointWaitTime);
        }
    }

    void CycleWaypointWhenClose(Vector3 nextWayPointPos) {
        if (Vector3.Distance(transform.position, nextWayPointPos) <= waypointTolerance) {
            character.GetNavMeshAgent().isStopped = true;
            nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            character.GetNavMeshAgent().isStopped = false;
        }
    }

    IEnumerator ChasePlayer() {
        state = State.chasing;

        while (distanceToPlayer >= currentWeaponRange) {
            character.SetDestination(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDrawGizmos() {
        // Draw attack sphere
        Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        // Draw chase sphere
        Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

} // EnemyAI
