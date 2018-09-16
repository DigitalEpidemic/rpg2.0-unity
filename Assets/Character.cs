using System;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase] // Selects top-level GameObject in editor
public class Character : MonoBehaviour {

    [Header("Animator")]
    [SerializeField] RuntimeAnimatorController animatorController;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField] Avatar characterAvatar;
    [SerializeField] [Range(0.1f, 1f)] float animatorForwardCap = 1f;

    [Header("Audio")]
    [SerializeField] float audioSourceSpatialBlend = 0f;

    [Header("Capsule Collider")]
    [SerializeField] Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
    [SerializeField] float colliderRadius = 0.2f;
    [SerializeField] float colliderHeight = 2.03f;

    [Header("Movement")]
    [SerializeField] float moveSpeedMultiplier = 0.7f;
    [SerializeField] float animationSpeedMultiplier = 1.5f;
    [SerializeField] float movingTurnSpeed = 360;
    [SerializeField] float stationaryTurnSpeed = 180;
    [SerializeField] float moveThreshold = 1f;

    [Header("Nav Mesh Agent")]
    [SerializeField] float navMeshAgentSteeringSpeed = 1.0f;
    [SerializeField] float navMeshAgentStoppingDistance = 1.3f;
    [SerializeField] float obstacleAvoidanceRadius = 0.5f;

    Vector3 clickPoint;
    GameObject walkTarget;
    NavMeshAgent navMeshAgent;
    Animator animator;
    Rigidbody myRigidbody;
    CapsuleCollider capsuleCollider;

    float turnAmount;
    float forwardAmount;
    bool isAlive = true;

    void Awake() {
        AddRequiredComponents();
    }

    void AddRequiredComponents() {
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.center = colliderCenter;
        capsuleCollider.radius = colliderRadius;
        capsuleCollider.height = colliderHeight;

        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = audioSourceSpatialBlend;

        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.avatar = characterAvatar;

        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = navMeshAgentSteeringSpeed;
        navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
        navMeshAgent.radius = obstacleAvoidanceRadius;
        navMeshAgent.autoBraking = false;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updatePosition = true;
    }

    void Update() {
        FixDeadSliding();

        if (!navMeshAgent.isOnNavMesh) {
            Debug.LogError(gameObject.name + " is not on the NavMesh!");
        } else if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive) {
            Move(navMeshAgent.desiredVelocity);
        } else {
            Move(Vector3.zero);
        }
    }

    public NavMeshAgent GetNavMeshAgent() {
        return navMeshAgent;
    }

    public void Kill() {
        isAlive = false;
    }

    public void SetDestination(Vector3 worldPos) {
        navMeshAgent.destination = worldPos;
    }

    public AnimatorOverrideController GetOverrideController() {
        return animatorOverrideController;
    }

    public float GetAnimSpeedMultiplier() {
        return animator.speed;
    }

    void Move(Vector3 movement) {
        SetForwardAndTurn(movement);

        ApplyExtraTurnRotation();
        UpdateAnimator();
    }

    void SetForwardAndTurn(Vector3 movement) {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired direction.
        if (movement.magnitude > moveThreshold) {
            movement.Normalize();
        }
        var localMove = transform.InverseTransformDirection(movement);

        turnAmount = Mathf.Atan2(localMove.x, localMove.z);
        forwardAmount = localMove.z;
    }

    void UpdateAnimator() {
        animator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        animator.speed = animationSpeedMultiplier;
    }

    void ApplyExtraTurnRotation() {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void OnAnimatorMove() {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.

        //if (Time.deltaTime > 0) {
        //    Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

        //    // we preserve the existing y part of the current velocity.
        //    velocity.y = myRigidbody.velocity.y;
        //    myRigidbody.velocity = velocity;
        //}


        navMeshAgent.velocity = (animator.deltaPosition * moveSpeedMultiplier * forwardAmount) / Time.deltaTime;
    }

    void FixDeadSliding() {
        if (!isAlive) {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.Move(Vector3.zero);
            capsuleCollider.radius = 0f;
        }
    }

} // Character