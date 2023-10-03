using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float m_JumpSpeed = 10f;
    [SerializeField] private float m_BaseGravity = 4f;
    [SerializeField] private float m_JumpHeight = 4f;	
    [SerializeField] private float m_JumpMin = 1f;	
    [SerializeField] private float m_MovementSpeed = 1f;	
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
    [Range(0, 1.0f)] [SerializeField] private float m_GroundCheckRadius = 0.5f;
    private bool m_IsGrounded = false;
    private bool m_Jump = false;
    private bool m_IsFalling = false;
    private bool m_FinishJump = false;
    private bool m_IsJumping = false;
    private bool m_MovingLeft = false;
    private bool m_MovingRight = false;
    private bool m_FastFalling = false;
    private float m_JumpStart = 0;	
    private Rigidbody2D m_Rigidbody2D;
    private Animator m_Animator;
    private Vector3 m_Force = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Awake() {
      m_Animator = GetComponent<Animator>();
      m_Rigidbody2D = GetComponent<Rigidbody2D>();
      m_Rigidbody2D.gravityScale = m_BaseGravity;
      checkAndUpdateIsGrounded();
      face(true, false);
    }

    void Update() {
      setAnimationState();
      if(m_IsGrounded && Input.GetKeyDown(KeyCode.Z)) {
        m_Jump = true;
      }
      if(Input.GetKeyUp(KeyCode.Z)) {
        m_FinishJump = true;
      }
      m_MovingLeft = Input.GetKey(KeyCode.LeftArrow);
      m_MovingRight = Input.GetKey(KeyCode.RightArrow);
    }

    void FixedUpdate() {
      checkAndUpdateIsGrounded();
      handleJump();
      move();
      float timeElapsed = Time.fixedDeltaTime;
      if (m_ForceTimer >= m_ForceTimerMax) {
        m_Force = new Vector3(0, 0, 0);
      } else {
        m_ForceTimer += timeElapsed;
      }
      m_Rigidbody2D.AddForce(m_Force * timeElapsed);
    }

    void setAnimationState() {
      m_Animator.SetBool("IsJumping", m_IsJumping);
      m_Animator.SetBool("IsGrounded", m_IsGrounded);
      m_Animator.SetBool("IsFalling", m_IsFalling);
    }

    void handleJump() {
      if(m_IsGrounded) {
        m_IsFalling = false;
        m_JumpStart = m_Rigidbody2D.position.y;
      }

      float jumpDistance = m_Rigidbody2D.position.y - m_JumpStart;

      if(m_IsJumping) {
        Vector3 velocity = m_Rigidbody2D.velocity; 
        bool reachedPeak = jumpDistance >= m_JumpHeight;
        bool pastMin = jumpDistance >= m_JumpMin;
        if(m_FinishJump && pastMin) {
          m_Rigidbody2D.velocity = new Vector3(velocity.x, 0, 0);
        }

        if((m_FinishJump || reachedPeak) && pastMin) {
          m_Rigidbody2D.gravityScale = m_BaseGravity;
          m_FinishJump = false;
          m_IsJumping = false;
          m_IsFalling = true;
        }
      }

      if(jumpDistance >= m_JumpHeight) {
        m_Rigidbody2D.gravityScale = m_BaseGravity * 4;
      } else {
        if(!m_IsJumping) {
          m_Rigidbody2D.gravityScale = m_BaseGravity;
        }
      }

      if (m_IsGrounded && m_Jump) {
        m_IsGrounded = false;
        m_Jump = false;
        m_FinishJump = false;
        m_IsJumping = true;
        m_Rigidbody2D.gravityScale = 0;
        m_JumpStart = m_Rigidbody2D.position.y;
        m_Rigidbody2D.velocity = new Vector3(0, m_JumpSpeed, 0);
      }
    }

    void move() {
      Vector3 currentVelocity = new Vector3(0, m_Rigidbody2D.velocity.y, 0);
      if(m_MovingLeft) {
        currentVelocity.x -= m_MovementSpeed;
      }
      if(m_MovingRight) {
        currentVelocity.x += m_MovementSpeed;
      }
      m_Rigidbody2D.velocity = currentVelocity;

      m_Animator.SetFloat("RunningSpeed", Mathf.Abs(currentVelocity.x));
      face(m_MovingRight, m_MovingLeft);
    }

    void face(bool movingRight, bool movingLeft) {
      // Switch the way the player is labelled as facing.
      Vector3 theScale = transform.localScale;
      if(!movingLeft && movingRight) {
        theScale.x = 1;
      } else if (movingLeft && !movingRight) {
        theScale.x = -1;
      }
      transform.localScale = theScale;
    }

    void checkAndUpdateIsGrounded() {
      Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, m_GroundCheckRadius, m_WhatIsGround);
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders[i].gameObject != gameObject) {
          m_IsGrounded = true;
        }
      }
    }

    public bool getIsGrounded() {
      return m_IsGrounded;
    }

    public void hit(Vector3 knockback) {
      m_FinishJump = true;
      m_Force = knockback;
      m_ForceTimer = 0;
    }
}
