using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour {

    Animator anim;
    NavMeshAgent navMeshAgent;

    bool walking;
    float speed;

    void Awake() {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButton(0)) { // TODO Change to input binding

            if (Physics.Raycast(ray, out hit)) {

                // TODO Add if hits enemy 
                speed += Time.deltaTime * 2f;


                if (Input.GetMouseButtonDown(0)) {
                    navMeshAgent.velocity = Vector3.zero;
                    navMeshAgent.isStopped = true;
                }

                if (Vector3.Distance(transform.position, hit.point) <= 0.8f) {
                    anim.speed = 1.5f;
                    if (speed >= 0.5f) {
                        speed -= Time.deltaTime * 4f;
                    } else {
                        anim.SetFloat("Speed", 0.5f);
                    }
                } else {
                    anim.speed = 1f;
                }

                walking = true;
                navMeshAgent.SetDestination(hit.point);
                navMeshAgent.isStopped = false;

            }
        } else {
            walking = false;
        }

        speed = Mathf.Clamp(speed, 0f, 1f);
        anim.SetFloat("Speed", speed);

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon) {
                walking = false;

            }
        } else {
            walking = true;
        }

        anim.SetBool("IsWalking", walking);
    }

} // ClickToMove