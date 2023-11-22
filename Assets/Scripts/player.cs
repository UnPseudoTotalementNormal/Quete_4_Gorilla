using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

public class player : MonoBehaviour
{

    private Rigidbody2D RB;
    private bool OnGround;

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!OnGround)
        {
            RB.velocity -= new Vector2(0, (float)0.03);
        }
        
        if (Left.action.inProgress)
        {
            
            RB.velocity = new Vector2((float)-5, RB.velocity.y); 
        }

        if (Right.action.inProgress)
        {
            RB.velocity = new Vector2((float)5, RB.velocity.y);
        }

        if (Shoot.action.inProgress)
        {
            balls.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1) * 100;
            Instantiate(balls, transform.position + new Vector3(1, 0, 0), transform.rotation);
        }
    }

    public void FeetTouched(Collider2D collision, bool touched) 
    {
        OnGround = touched;
    }

    public void JumpFunction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && OnGround)
        {
            RB.velocity += Vector2.up * 7;
        }
        
    }
}
