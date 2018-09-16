using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class PlayerControl : MonoBehaviour {

    Animator anim;
    NavMeshAgent navMeshAgent;
    CameraRaycaster cameraRaycaster;

    bool walking;
    float speed = 1.0f;

    void Awake() {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
    }

    void Update() {
        if (Input.GetMouseButton(0)) { // TODO Change to input binding

            switch (cameraRaycaster.layerHit) {
                case Layer.Enemy: // Enemy layer
                    Debug.Log("TODO: Implement attacking enemy.");
                    break;
                default: // Every layer that is left
                    MovePlayer();
                    break;
            }

        }
        AnimatePlayer();
    }

    void MovePlayer() {
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

} // PlayerControl