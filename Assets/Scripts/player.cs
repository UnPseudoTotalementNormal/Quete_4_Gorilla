using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    private GameObject gamemanager;
    private bool _myturn = false;
    private enum STATE
    {
        Normal,
        Charging,
        Aiming,
    }

    private STATE _state = STATE.Normal;

    private Rigidbody2D RB;
    private bool OnGround;
    private Vector2 _mousepos;
    private float _mouseangle;

    private float _currentforce = 0;
    [SerializeField] private float _maxforce = 15;
    [SerializeField] private float _chargingspeed = 10; //per sec

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot, Aim, MousePos;

    private float _sprite_angle;
    private Transform _monkesprite;

    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        RB = GetComponent<Rigidbody2D>();
        _monkesprite = transform.Find("MonkeSprite");
    }

    void Update()
    {
        Transform monkeform = _monkesprite.GetComponent<Transform>();
        float currrot = monkeform.transform.localRotation.eulerAngles.z;
        float nextrot = Mathf.LerpAngle(currrot, _sprite_angle, 6f * Time.deltaTime);
        monkeform.transform.rotation = Quaternion.Euler(Vector3.forward * nextrot);
        monkeform.transform.SetLocalPositionAndRotation(new Vector3(0, 0.2f + 0.2f * Mathf.Sin(Time.time * 5), 0), monkeform.transform.rotation);
        
    }

    void FixedUpdate()
    {
        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);

        _sprite_angle = 0;

        turncheck();
        if (!_myturn) 
        {
            _state = STATE.Normal;
            _currentforce = 0;
            transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(false);
            return; 
        }

        switch (_state)
        {
            case STATE.Normal:
                if (Left.action.inProgress)
                {
                    gamemanager.GetComponent<GameScript>().ActionTimer();
                    RB.velocity = new Vector2(-5f, RB.velocity.y);
                    _monkesprite.GetComponent<SpriteRenderer>().flipX = true;
                    _sprite_angle = 30;
                }

                if (Right.action.inProgress)
                {
                    gamemanager.GetComponent<GameScript>().ActionTimer();
                    RB.velocity = new Vector2(5f, RB.velocity.y);
                    _monkesprite.GetComponent<SpriteRenderer>().flipX = false;
                    _sprite_angle = -30;
                }
                break;

            case STATE.Charging:
                _currentforce += _chargingspeed * Time.fixedDeltaTime;
                transform.Find("Canvas").Find("ChargingBar").Find("ChargingMask").GetComponent<RectMask2D>().padding = new Vector4(0, 0, ((_maxforce - _currentforce) / (_maxforce - 0)) * 64, 0);
                if (_currentforce >= _maxforce)
                {
                    _state = STATE.Normal;
                    ShootFunction();
                }
                break;
            case STATE.Aiming:
                transform.Find("Canvas").Find("ChargingBar").Find("ChargingMask").GetComponent<RectMask2D>().padding = new Vector4(0, 0, 64, 0);
                transform.Find("Canvas").transform.rotation = Quaternion.Euler(Vector3.forward * _mouseangle);
                break;
        }
    }

    public void FeetTouched(Collider2D collision, bool touched) 
    {
        OnGround = touched;
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn) { return; }
        if (_state == STATE.Normal)
        {
            if (ctx.phase == InputActionPhase.Started && OnGround)
            {
                gamemanager.GetComponent<GameScript>().ActionTimer();
                RB.velocity += Vector2.up * 14;
            }
        }
    }
    public void ShootInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn) { return; }

        if (ctx.phase == InputActionPhase.Started && _state == STATE.Aiming)
        {
            _currentforce = 0;
            _state = STATE.Charging;
        }
        if (ctx.phase == InputActionPhase.Canceled && _state == STATE.Charging)
        {
            _state = STATE.Normal;
            ShootFunction();
        }
    }

    public void AimInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn) { return; }
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                _state = STATE.Aiming;
                transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(true);
                break;
            case InputActionPhase.Canceled:
                _state = STATE.Normal;
                _currentforce = 0;
                transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(false);
                break;
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

        Vector2 Ppos = (Vector2)GetComponent<Transform>().position;
        _mouseangle = Mathf.Atan2(_mousepos.y - Ppos.y, _mousepos.x - Ppos.x);
        _mouseangle *= Mathf.Rad2Deg; 
    }

    private void turncheck()
    {
        _myturn = (gamemanager.GetComponent<GameScript>().Memberturn == this.gameObject);
    }
}
