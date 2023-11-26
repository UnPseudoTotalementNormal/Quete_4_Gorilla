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

    private enum STATE
    {
        Normal,
        Charging
    }

    private STATE _state = STATE.Normal;

    private Rigidbody2D RB;
    private bool OnGround;
    private Vector2 _mousepos;

    private float _currentforce = 0;
    private float _maxforce = 20;
    private float _charingspeed = 10; //per sec

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot, Aim, MousePos;

    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);

        turncheck();
        if (!_myturn) 
        {
            switch (_state)
            {
                case STATE.Charging:
                    ShootFunction();
                    break;
            }
            _state = STATE.Normal;
            return; 
        }

        switch (_state)
        {
            case STATE.Normal:
                if (Left.action.inProgress)
                {
                    gamemanager.GetComponent<GameScript>().ActionTimer();
                    RB.velocity = new Vector2(-5f, RB.velocity.y);
                }

                if (Right.action.inProgress)
                {
                    gamemanager.GetComponent<GameScript>().ActionTimer();
                    RB.velocity = new Vector2(5f, RB.velocity.y);
                }
                break;

            case STATE.Charging:
                _currentforce += _charingspeed * Time.fixedDeltaTime;
                if (_currentforce >= _maxforce)
                {
                    _state = STATE.Normal;
                    ShootFunction();
                }
                break;
        }
    }

    public void FeetTouched(Collider2D collision, bool touched) 
    {
        OnGround = touched;
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if (_state == STATE.Normal)
        {
            if (ctx.phase == InputActionPhase.Started && OnGround && _myturn)
            {
                gamemanager.GetComponent<GameScript>().ActionTimer();
                RB.velocity += Vector2.up * 14;
            }
        }
    }
    public void ShootInput(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && _myturn)
        {
            _currentforce = 0;
            _state = STATE.Charging;
        }
        if (ctx.phase == InputActionPhase.Canceled && _myturn)
        {
            _state = STATE.Normal;
            ShootFunction();
        }
    }

    private void ShootFunction()
    {
        Vector2 Ppos = (Vector2)GetComponent<Transform>().position;
        Vector2 shootvector = _mousepos - Ppos;
        GameObject newball;
        newball = Instantiate(balls, transform.position, transform.rotation);

        newball.GetComponent<BallScript>().SetAngle(shootvector.normalized, _currentforce);
        gamemanager.GetComponent<GameScript>().EndTurn(newball);
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
