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
    public void ShootFunction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            float shootX = GetComponent<Transform>().position.x;
            float shootY = GetComponent<Transform>().position.y;
            Vector2 shootvector = _mousepos - new Vector2(shootX, shootY);
            GameObject newball;
            print(_mousepos.x);
            if (_mousepos.x >= 0.0f) { newball = Instantiate(balls, transform.position + new Vector3(1, 0, 0), transform.rotation); }
            else { newball = Instantiate(balls, transform.position + new Vector3(-1, 0, 0), transform.rotation); }

            newball.GetComponent<BallScript>().SetAngle(shootvector);
        }
    }

    public void GetMousePosition(InputAction.CallbackContext ctx)
    {
        _mousepos = ctx.ReadValue<Vector2>();
        _mousepos = Camera.main.ScreenToWorldPoint(_mousepos);
    }
}
