using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using System.Runtime.CompilerServices;

public class player : MonoBehaviour
{

    private Rigidbody2D RB;
    private bool OnGround;
    private Vector2 _mousepos;

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot, MousePos;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {   
        if (Left.action.inProgress)
        {
            
            RB.velocity = new Vector2(-5f, RB.velocity.y); 
        }

        if (Right.action.inProgress)
        {
            RB.velocity = new Vector2(5f, RB.velocity.y);
        }

        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);
    }

    public void FeetTouched(Collider2D collision, bool touched) 
    {
        OnGround = touched;
    }

    public void JumpFunction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && OnGround)
        {
            RB.velocity += Vector2.up * 14;
        }
        
    }
    public void ShootFunction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            Vector2 Ppos = (Vector2)GetComponent<Transform>().position;
            Vector2 shootvector = _mousepos - Ppos;
            GameObject newball;
            newball = Instantiate(balls, transform.position + new Vector3(1, 0, 0), transform.rotation);

            newball.GetComponent<BallScript>().SetAngle(shootvector.normalized, 10);
        }
    }

    public void GetMousePosition(InputAction.CallbackContext ctx)
    {
        _mousepos = ctx.ReadValue<Vector2>();
        _mousepos = Camera.main.ScreenToWorldPoint(_mousepos);
    }
}
