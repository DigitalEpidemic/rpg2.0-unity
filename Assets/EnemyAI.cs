using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    [SerializeField] float chaseRadius = 6f;

    NavMeshAgent navMeshAgent;
    Rigidbody myRigidbody;
    PlayerControl player;

    float distanceToPlayer;
    float currentWeaponRange = 2f;

    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerControl>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        bool inWeaponRadius = distanceToPlayer <= currentWeaponRange;
        bool inChaseRadius = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
        bool outsideChaseRadius = distanceToPlayer > chaseRadius;

        if (inWeaponRadius) {
            StopAllCoroutines();
            transform.LookAt(player.transform);
            navMeshAgent.SetDestination(Vector3.zero);
            navMeshAgent.velocity = Vector3.zero;
            Debug.Log(gameObject.name + ": Attacking player!");
        }

        if (outsideChaseRadius) {
            StopAllCoroutines();
            navMeshAgent.SetDestination(transform.position); // Temporarily stop enemy when outside of radius
            //StartCoroutine(Patrol());
        }

        if (inChaseRadius) {
            StopAllCoroutines();
            StartCoroutine(ChasePlayer());
        }
    }

    IEnumerator ChasePlayer() {
        //state = State.chasing;
        while (distanceToPlayer >= currentWeaponRange) {
            navMeshAgent.SetDestination(player.transform.position);
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
