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
using static UnityEngine.GraphicsBuffer;
using System.Linq;

public class player : MonoBehaviour
{
    private GameObject gamemanager;
    private bool _myturn = false;
    public bool played_this_turn = false;
    private enum STATE
    {
        Normal,
        Charging,
        Aiming,
        Escaping,
    }

    private STATE _state = STATE.Normal;

    private Rigidbody2D RB;
    private bool OnGround;
    private Vector2 _mousepos;
    private float _mouseangle;
    private float _oldmouseangle; //before charging

    private float _currentforce = 0;
    [SerializeField] public float _maxforce = 15;
    [SerializeField] public float _chargingspeed = 10; //per sec

    [SerializeField] private GameObject balls;
    [SerializeField] private InputActionReference Jump, Left, Right, Shoot, Aim, MousePos;

    private float _sprite_angle;
    private Transform _monkesprite;

    private List<Vector2> _se_oldpos = new List<Vector2>(); //se == shoot emulate
    private Vector2 _se_position;
    private Vector2 _se_velocity;

    public Dictionary<String, int> upgrade_list = new Dictionary<String, int>();
    public float walking_speed = 5;
    public float jump_force = 14;
    public float explosion_radius = 1;
    public bool triple_shot = false;
    public int multi_shot = 1;
    public bool visual_aiming = false;
    public bool continue_aiming = false;

    private int shot_this_turn = 0;

    [SerializeField] private GameObject _se_line;
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        RB = GetComponent<Rigidbody2D>();
        _monkesprite = transform.Find("MonkeSprite");
        _se_line = Instantiate(_se_line);
    }

    void Update()
    {
        Transform monkeform = _monkesprite.GetComponent<Transform>();
        float currrot = monkeform.transform.localRotation.eulerAngles.z;
        float nextrot = Mathf.LerpAngle(currrot, _sprite_angle, 6f * Time.deltaTime);
        monkeform.transform.rotation = Quaternion.Euler(Vector3.forward * nextrot);
        monkeform.transform.SetLocalPositionAndRotation(new Vector3(0, 0.2f + 0.2f * Mathf.Sin(Time.time * 5), 0), monkeform.transform.rotation);
        Sprite newsprite = Resources.Load<Sprite>("Sprites/MONKE" + GetComponent<HealthComponent>().health.ToString());
        _monkesprite.GetComponent<SpriteRenderer>().sprite = newsprite;

    }

    void FixedUpdate()
    {
        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);

        _sprite_angle = 0;
        _se_line.SetActive(false);

        turncheck();
        if (!_myturn) 
        {
            _state = STATE.Normal;
            _currentforce = 0;
            shot_this_turn = 0;
            transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(false);
            return; 
        }
        played_this_turn = true;

        switch (_state)
        {
            case STATE.Normal:
                if (Left.action.inProgress)
                {
                    WalkLeft();
                }

                if (Right.action.inProgress)
                {
                    WalkRight();
                }
                break;
            case STATE.Charging:
                if (continue_aiming) _oldmouseangle = _mouseangle;
                if (visual_aiming)
                {
                    _se_line.SetActive(true);
                    TestShooting();
                }
                _currentforce += _chargingspeed * Time.fixedDeltaTime;
                transform.Find("Canvas").Find("ChargingBar").Find("ChargingMask").GetComponent<RectMask2D>().padding = new Vector4(0, 0, ((_maxforce - _currentforce) / (_maxforce - 0)) * 64, 0);
                transform.Find("Canvas").transform.rotation = Quaternion.Euler(Vector3.forward * _oldmouseangle);
                if (_currentforce >= _maxforce)
                {
                    ShootFunction();
                }
                break;
            case STATE.Aiming:
                transform.Find("Canvas").transform.rotation = Quaternion.Euler(Vector3.forward * _mouseangle);
                break;
            case STATE.Escaping:
                transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(false);
                if (Left.action.inProgress)
                {
                    WalkLeft();
                }
                if (Right.action.inProgress)
                {
                    WalkRight();
                }
                break;
        }
    }

    private void WalkLeft()
    {
        RB.velocity = new Vector2(-walking_speed, RB.velocity.y);
        _monkesprite.GetComponent<SpriteRenderer>().flipX = true;
        _sprite_angle = 30;
    }

    private void WalkRight()
    {
        RB.velocity = new Vector2(walking_speed, RB.velocity.y);
        _monkesprite.GetComponent<SpriteRenderer>().flipX = false;
        _sprite_angle = -30;
    }
    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn) { return; }
        if (_state == STATE.Normal || _state == STATE.Escaping)
        {
            if (ctx.phase == InputActionPhase.Started && OnGround)
            {
                _monkesprite.Find("FirePart").Find("Burst").GetComponent<ParticleSystem>().Play();
                RB.velocity += Vector2.up * jump_force;
            }
        }
    }

    private void TestShooting()
    {
        DrawDebugShooting();
        _se_position = RB.position;
        _se_oldpos.Clear();
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(_oldmouseangle * Mathf.Deg2Rad), (float)Math.Sin(_oldmouseangle * Mathf.Deg2Rad));
        _shoot_vector *= _currentforce;
        _se_velocity = _shoot_vector;

        for (int i = 0; i < 200; i++)
        {
            _se_oldpos.Add(_se_position);

            _se_position += _se_velocity * Time.fixedDeltaTime;
            _se_velocity += new Vector2(0, -9.80665f) * Time.fixedDeltaTime;
            _se_velocity += gamemanager.GetComponent<GameScript>().wind * Time.fixedDeltaTime;

            var _raycast = Physics2D.CircleCast(_se_position, 0.5f, _se_velocity.normalized, 0.5f);

            if (_raycast.collider != null)
            {
                if (_raycast.collider.GetComponent<BoxCollider2D>() != null && _raycast.transform.tag == "Map")
                {
                    break;
                }
            }
        }
    }
    private void DrawDebugShooting()
    {
        _se_line.transform.position = transform.position;
        LineRenderer line =  _se_line.GetComponent<LineRenderer>();
        line.positionCount = 0;
        int pointnum = Mathf.Clamp(30, 0, _se_oldpos.Count() - 1);
        if (pointnum <= 0) return;
        line.positionCount = pointnum;
        for (int i = 0; i < pointnum; i++)
        {
            Debug.DrawLine(_se_oldpos[i], _se_oldpos[i + 1], Color.red);
            line.SetPosition(i, _se_oldpos[i]);
        }
    }

    public void FeetTouched(Collider2D collision, bool touched) 
    {
        OnGround = touched;
    }

    public void ShootInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn) { return; }

        if (ctx.phase == InputActionPhase.Started && _state == STATE.Aiming)
        {
            _currentforce = 0;
            _state = STATE.Charging;
            _oldmouseangle = _mouseangle;
        }
        if (ctx.phase == InputActionPhase.Canceled && _state == STATE.Charging)
        {
            ShootFunction();
        }
    }

    public void AimInput(InputAction.CallbackContext ctx)
    {
        if (!_myturn || _state == STATE.Charging || _state == STATE.Escaping) { return; }
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                _state = STATE.Aiming;
                transform.Find("Canvas").Find("ChargingBar").Find("ChargingMask").GetComponent<RectMask2D>().padding = new Vector4(0, 0, 64, 0);
                transform.Find("Canvas").transform.rotation = Quaternion.Euler(Vector3.forward * _mouseangle);
                transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(true);
                break;
            case InputActionPhase.Canceled:
                _state = STATE.Normal;
                _currentforce = 0;
                transform.Find("Canvas").Find("ChargingBar").gameObject.SetActive(false);
                break;
        }
    }

    private void ShootFunction(bool turn_actions = true)
    {
        Vector2 shootvector = new Vector2(Mathf.Cos((_oldmouseangle) * Mathf.PI / 180), Mathf.Sin((_oldmouseangle) * Mathf.PI / 180));

        GameObject newball;
        newball = Instantiate(balls, transform.position, transform.rotation);
        newball.GetComponent<BallScript>().SetAngle(shootvector.normalized, _currentforce);
        newball.GetComponent<BallScript>().SetRadius(explosion_radius);

        if (triple_shot)
        {
            Vector2 tripleshootvector = new Vector2(Mathf.Cos((_oldmouseangle + 10) * Mathf.PI / 180), Mathf.Sin((_oldmouseangle + 10) * Mathf.PI / 180));
            GameObject newball2;
            newball2 = Instantiate(balls, transform.position, transform.rotation);
            newball2.GetComponent<BallScript>().SetAngle(tripleshootvector.normalized, _currentforce);
            newball2.GetComponent<BallScript>().SetRadius(explosion_radius);

            tripleshootvector = new Vector2(Mathf.Cos((_oldmouseangle - 10) * Mathf.PI / 180), Mathf.Sin((_oldmouseangle - 10) * Mathf.PI / 180));
            GameObject newball3;
            newball3 = Instantiate(balls, transform.position, transform.rotation);
            newball3.GetComponent<BallScript>().SetAngle(tripleshootvector.normalized, _currentforce);
            newball3.GetComponent<BallScript>().SetRadius(explosion_radius);
        }

        if (turn_actions)
        {
            shot_this_turn += 1;
            if (shot_this_turn >= multi_shot)
            {
                _state = STATE.Escaping;
                gamemanager.GetComponent<GameScript>().ActionTimer();
                gamemanager.GetComponent<GameScript>().FollowThis(newball, this.gameObject);
            }
            else
            {
                _state = STATE.Normal;
            }
        }
        else
        {
            _state = STATE.Normal;
        }
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
