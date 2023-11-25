using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    private GameObject gamemanager;
    private bool _myturn = false;

    private Rigidbody2D RB;
    private bool OnGround;
    private Vector2 _mousepos;

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot, MousePos;

    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        RB = GetComponent<Rigidbody2D>();
        //SceneManager.LoadScene("Scenes/MainMenu");
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);

        turncheck();
        if (!_myturn) { return; }

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
        if (ctx.phase == InputActionPhase.Started && OnGround && _myturn)
        {
            RB.velocity += Vector2.up * 14;
        }
        
    }
    public void ShootFunction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && _myturn)
        {
            Vector2 Ppos = (Vector2)GetComponent<Transform>().position;
            Vector2 shootvector = _mousepos - Ppos;
            GameObject newball;
            newball = Instantiate(balls, transform.position, transform.rotation);

            newball.GetComponent<BallScript>().SetAngle(shootvector.normalized, 10);

            gamemanager.GetComponent<GameScript>().EndTurn();
        }
    }

    public void GetMousePosition(InputAction.CallbackContext ctx)
    {
        _mousepos = ctx.ReadValue<Vector2>();
        _mousepos = Camera.main.ScreenToWorldPoint(_mousepos);
    }

    private void turncheck()
    {
        _myturn = (gamemanager.GetComponent<GameScript>().Memberturn == this.gameObject);
    }
}
