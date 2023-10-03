using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody;
    private bool m_IsDownAttack = false;
    private bool m_IsUpAttack = false;
    private PlayerMovement m_PlayerMovement;
    [SerializeField] private float m_KnockbackForce = 1000;

    void Awake() {
      m_Animator = GetComponent<Animator>();
      m_Rigidbody = GetComponent<Rigidbody2D>();
      m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
      if(Input.GetKeyDown(KeyCode.X)) {
        m_Animator.SetTrigger("Attack");
      }
      m_IsDownAttack = Input.GetKey(KeyCode.DownArrow);
      m_IsUpAttack = Input.GetKey(KeyCode.UpArrow);
    }

    public void Attack() {
        string attackTag = "PlayerAttack";
        Vector3 knockback = new Vector3(0, 0, 0);

        if(m_IsDownAttack && !m_PlayerMovement.getIsGrounded()) {
            attackTag = "PlayerAttackDown";
        } else if(m_IsUpAttack) {
            attackTag = "PlayerAttackUp";
        }

        GameObject attack = GameObject.FindGameObjectsWithTag(attackTag)[0];
        attack.GetComponent<PlayerAttack>().Attack();

        if (m_IsDownAttack && !m_PlayerMovement.getIsGrounded())
        {
            knockback.y = m_KnockbackForce;
        }
        else if (!m_IsUpAttack)
        {
            bool knockbackLeft = attack.transform.position.x > transform.position.x;
            knockback.x = knockbackLeft ? -m_KnockbackForce : m_KnockbackForce;
        }

        m_PlayerMovement.hit(knockback);
    }

    public void FinishAttack() {
        foreach(GameObject attack in GameObject.FindGameObjectsWithTag("PlayerAttack")) {
          attack.GetComponent<PlayerAttack>().FinishAttack();
        }
        foreach(GameObject attack in GameObject.FindGameObjectsWithTag("PlayerAttackUp")) {
          attack.GetComponent<PlayerAttack>().FinishAttack();
        }
        foreach(GameObject attack in GameObject.FindGameObjectsWithTag("PlayerAttackDown")) {
          attack.GetComponent<PlayerAttack>().FinishAttack();
        }
    }
}
