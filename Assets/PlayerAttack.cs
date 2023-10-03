using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    private SpriteRenderer m_SpriteRenderer;
    private bool m_Showing = false;

    void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_SpriteRenderer.enabled = m_Showing;
    }

    public void Attack() {
        m_Showing = true;
    }

    public void FinishAttack() {
        m_Showing = false;
    }
}
