using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class EnnemiScript : MonoBehaviour
{
    private GameObject gamemanager;
    private bool _myturn = false;
    private enum STATE
    {
        Idle,
        TestingShooting,
        Moving,
    }

    private bool OnGround;
    private bool OnWallRight;
    private bool OnWallLeft;

    private bool HoleRight;
    private bool HoleLeft;
    private int HoleRightBuffer = 0;
    private int HoleLeftBuffer = 0;

    private bool _walk_oposite_dir = false;
    private bool _jumped = false;
    private float _old_jumpx;
    private float _walking_timer = 0f;
    private float _walking_max_timer = 2f;

    [SerializeField] private GameObject balls;

    [SerializeField] private float _maxforce = 15;
    [SerializeField] private float _chargingspeed = 10; //per sec

    private Rigidbody2D RB;
    private STATE _state = STATE.Idle;

    private float _shoot_timer = 0;
    private float _shoot_max_timer = 1f; //in seconds

    private float _angle = 0;
    private float _shoot_force;

    private List<Vector2> _se_oldpos = new List<Vector2>(); //se == shoot emulate
    private Vector2 _se_position;
    private Vector2 _se_velocity;

    private GameObject _target;

    private int _se_iteration = 400;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Monke1");
    }
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
    }
    private void FixedUpdate()
    {
        if (RB.velocity.magnitude < 1) RB.velocity -= new Vector2(RB.velocity.x, 0);
        else RB.velocity -= new Vector2(0.5f * RB.velocity.normalized.x, 0);

        if (!OnGround)
        {
            HoleLeftBuffer = 0;
            HoleRightBuffer = 0;
        }

        turncheck();
        if (!_myturn) 
        {
            _state = STATE.Idle;
            return; 
        }

        switch (_state)
        {
            case STATE.Idle:
                WaitShooting();
                break;

            case STATE.TestingShooting:
                for (int i = 0; i < 200; i++)
                {
                    StartCoroutine(TestShooting());
                    if (_state != STATE.TestingShooting) break;
                }
                break;
            case STATE.Moving:
                WaitWalkingStop();
                if (_jumped && OnGround && RB.velocity.y <= 0)
                {
                    if (Mathf.Abs(_old_jumpx - transform.position.x) < 0.3f)
                    {
                        _old_jumpx = 0;
                        _jumped = false;
                        _walk_oposite_dir = !_walk_oposite_dir;
                    }
                }
                if (_target.transform.position.x - transform.transform.position.x > 0)
                {
                    if (_walk_oposite_dir) WalkLeft();
                    else WalkRight();
                }
                else
                {
                    if (_walk_oposite_dir) WalkRight();
                    else WalkLeft();
                }
                break;
        }
    }

    private void WalkLeft()
    {
        if ((OnWallLeft || HoleLeft) && RB.velocity.y <= 0) JumpFunction();
        RB.velocity = new Vector2(-5f, RB.velocity.y);
    }

    private void WalkRight()
    {
        if ((OnWallRight || HoleRight) && RB.velocity.y <= 0) JumpFunction();
        RB.velocity = new Vector2(5f, RB.velocity.y);
    }

    private void JumpFunction()
    {
        if (OnGround)
        {
            _old_jumpx = transform.position.x;
            _jumped = true;
            RB.velocity = Vector2.up * 14;
            if (_walking_timer > 1) 
            {
                _walking_timer = 1;
            }
        }
    }
    private void WaitShooting()
    {
        DrawDebugShooting();
        _shoot_timer += Time.deltaTime;
        if (_shoot_timer > _shoot_max_timer ) 
        {
            StartTestShooting();
        }
    }

    private void WaitWalkingStop()
    {
        _walking_timer += Time.deltaTime;
        if (_walking_timer > _walking_max_timer )
        {
            StartTestShooting();
        }
    }
    IEnumerator TestShooting()
    {
        DrawDebugShooting();
        _angle += 0.0015f;
        if (_angle > 3 * Math.PI/2)
        {
            _angle = (float)Math.PI/2.2f;
            _shoot_force += 1;
        }
        if (_shoot_force >= _maxforce) 
        { 
            _state = STATE.Moving;
            yield return null;
        }
        _se_position = RB.position;
        _se_oldpos.Clear();
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        _shoot_vector *= _shoot_force;
        _se_velocity = _shoot_vector;
        
        for (int i = 0; i < _se_iteration; i++)
        {
            _se_oldpos.Add(_se_position);

            _se_position += _se_velocity * Time.fixedDeltaTime;
            _se_velocity += new Vector2(0, -9.80665f) * Time.fixedDeltaTime;

            var _raycast = Physics2D.CircleCast(_se_position, 0.5f, _se_velocity.normalized, 0.5f);

            if (_raycast.collider != null)
            {
                if (_raycast.collider.GetComponent<BoxCollider2D>() != null && _raycast.transform.tag == "Map") 
                {
                    break;
                }
            }

            if (_target.GetComponent<CapsuleCollider2D>().OverlapPoint(_se_position))
            {
                Shoot(_shoot_vector);
                _state = STATE.Idle;
                break;
            }
        }
        yield return null;
    }
    private void StartTestShooting()
    {
        _angle = (float)Math.PI / 2;
        _shoot_timer = 0;
        _shoot_force = 5;
        _walking_timer = 0;
        _state = STATE.TestingShooting;
        
    }
    private void DrawDebugShooting()
    {
        for (int i = 0; i < _se_oldpos.Count() - 1; i++)
        {
            Debug.DrawLine(_se_oldpos[i], _se_oldpos[i + 1], Color.red);
        }
    }

    private void Shoot(Vector2 shootvector)
    {
        GameObject newball = Instantiate(balls, transform.position, transform.rotation);
        newball.GetComponent<BallScript>().SetAngle(shootvector, 1.008f);

        gamemanager.GetComponent<GameScript>().EndTurn(newball);
    }

    private void turncheck()
    {
        _myturn = (gamemanager.GetComponent<GameScript>().Memberturn == this.gameObject);
    }
    public void FeetTouched(Collider2D collision, bool touched)
    {
        OnGround = touched;
    }

    public void LeftSideTouched(Collider2D collision, bool touched)
    {
        if (collision.tag == "Map" && touched)
        {
            OnWallLeft = touched;
        }
        else
        {
            OnWallLeft = touched;
        }
    }

    public void RightSideTouched(Collider2D collision, bool touched)
    {
        if (collision.tag == "Map" && touched)
        {
            OnWallRight = touched;
        }
        else
        {
            OnWallRight = touched;
        }
    }

    public void LeftSideHole(bool hole)
    {
        if (hole)
        {
            HoleLeftBuffer++;
            if (HoleLeftBuffer > 3)
            {
                HoleLeft = true;
                HoleLeftBuffer = 0;
            }
        }
        else
        {
            HoleLeft = false;
            HoleLeftBuffer = 0;
        }
         
    }

    public void RightSideHole(bool hole)
    {
        if (hole)
        {
            HoleRightBuffer++;
            if (HoleRightBuffer > 3)
            {
                HoleRight = true;
                HoleRightBuffer = 0;
            }
        }
        else
        {
            HoleRight = false;
            HoleRightBuffer = 0;
        }
    }
}
