using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    float m_MovingTurnSpeed = 360;
    float m_StationaryTurnSpeed = 180;
    float m_AnimSpeedMultiplier = 1f;

    float m_TurnAmount;
    float m_ForwardAmount;

    Animator m_Animator;

    // Use this for initialization
    void Start () {
        m_Animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move(Vector3 move) {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    void UpdateAnimator(Vector3 move) {
        // update the animator parameters
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);

        if (move.magnitude > 0) {
            m_Animator.speed = m_AnimSpeedMultiplier;
        } else {
            m_Animator.speed = 1;
        }
        
    }

    void ApplyExtraTurnRotation() {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }



} // CharacterMovement
