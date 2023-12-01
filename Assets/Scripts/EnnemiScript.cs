using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor;

public class EnnemiScript : MonoBehaviour
{
    private GameObject gamemanager;
    private bool _myturn = false;
    public bool played_this_turn = false;
    private enum STATE
    {
        Idle,
        TestingShooting,
        Moving,
        Escaping,
        SHOOTWALL,
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
    private float _walking_max_timer = 1f;
    private float _shootwall_timer = 0f;
    private float _shootwall_max_timer = 0.5f;

    [SerializeField] private GameObject balls;

    [SerializeField] private float _maxforce = 15;
    [SerializeField] private float _chargingspeed = 10; //per sec

    private float _rand_angle = 15;

    private Rigidbody2D RB;
    private STATE _state = STATE.Idle;

    private float _shoot_timer = 0;
    private float _shoot_max_timer = 1f; //in seconds

    private float _angle = 0;
    private float _shoot_force;

    private List<Vector2> _se_oldpos = new List<Vector2>(); //se == shoot emulate

    private GameObject _target;
    private Collider2D _shootingwall;

    private int _se_iteration = 200;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Monke1");
    }
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
    }

    private void Update()
    {
        GameObject _bloonsprite = transform.Find("BloonSprite").gameObject;
        _bloonsprite.transform.SetLocalPositionAndRotation(new Vector3(0, 0.2f + 0.2f * Mathf.Sin(Time.time * 5), 0), _bloonsprite.transform.rotation);
        Sprite newsprite = Resources.Load<Sprite>("Sprites/Bloons" + GetComponent<HealthComponent>().health.ToString());
        _bloonsprite.GetComponent<SpriteRenderer>().sprite = newsprite;
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
            _walk_oposite_dir = false;
            _jumped = false;
            HoleLeftBuffer = 0;
            HoleRightBuffer = 0;
            return; 
        }
        played_this_turn = true;

        switch (_state)
        {
            case STATE.Idle:
                WaitShooting();
                break;

            case STATE.TestingShooting:
                alltestshooting(60);
                break;
            case STATE.SHOOTWALL:
                _shootwall_timer += 1 * Time.deltaTime;
                if (_shootwall_timer >= _shootwall_max_timer)
                {

                    allwallshooting(60);
                }
                if (_target.transform.position.x - transform.transform.position.x > 0)
                {
                    WalkLeft();
                }
                else
                {
                    WalkRight();
                }
                break;
            case STATE.Moving:
                if (gamemanager.GetComponent<GameScript>().timer <= 1.5)
                {
                    _state = STATE.Escaping;
                    break;
                }
                WaitWalkingStop();
                if (_jumped && OnGround && RB.velocity.y <= 0)
                {
                    if (Mathf.Abs(_old_jumpx - transform.position.x) < 0.3f)
                    {
                        gethighwall();
                        _shootwall_timer = 0;
                        _old_jumpx = 0;
                        _jumped = false;
                        //_walk_oposite_dir = !_walk_oposite_dir;
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
            case STATE.Escaping:
                if (_target.transform.position.x - transform.transform.position.x > 0)
                {
                    WalkLeft();
                }
                else
                {
                    WalkRight();
                }
                break;
        }
    }

    private void gethighwall()
    {
        Vector2 testwallpos = (Vector2)transform.position;
        Collider2D wallcollider = null;
        int side = -1;
        if (_target.transform.position.x - transform.transform.position.x > 0) side = 1;
        for (int i = 0; i < 50; i++)
        {
            var raycast = Physics2D.Raycast(testwallpos, Vector2.right * side, 3);
            if (raycast.collider == null && wallcollider != null)
            {
                _state = STATE.SHOOTWALL;
                _shootingwall = wallcollider;
                break;
            }
            else
            {
                wallcollider = raycast.collider;
                testwallpos += new Vector2(0, 1);
            }
        }
    }

    private void allwallshooting(int coroutine_num)
    {
        float base_offset_angle = 180 / coroutine_num;
        _angle = (float)Math.PI / 2f;
        _shoot_force += 1;
        if (_shoot_force >= _maxforce)
        {
            _shoot_force = 5;
        }

        for (int i = 0; i <= coroutine_num; i++)
        {
            float offset_angle = 0;
            if (_shootingwall.transform.position.x - transform.transform.position.x > 0)
            {
                offset_angle = base_offset_angle * i;
            }
            else offset_angle = base_offset_angle * i + 180;
            StartCoroutine(WallShooting(_angle - (offset_angle * Mathf.Deg2Rad)));
            if (_state != STATE.SHOOTWALL) break;
        }
    }

    IEnumerator WallShooting(float angle_test)
    {
        _se_oldpos.Clear();
        Vector2 _se_c_position;
        Vector2 _se_c_velocity;
        _se_c_position = RB.position;
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(angle_test), (float)Math.Sin(angle_test));
        _shoot_vector *= _shoot_force;
        _se_c_velocity = _shoot_vector;

        for (int i = 0; i < _se_iteration; i++)
        {
            _se_oldpos.Add(_se_c_position);
            DrawDebugShooting();

            _se_c_position += _se_c_velocity * Time.fixedDeltaTime;
            _se_c_velocity += new Vector2(0, -9.80665f) * Time.fixedDeltaTime;
            _se_c_velocity += gamemanager.GetComponent<GameScript>().wind * Time.fixedDeltaTime;

            var _raycast = Physics2D.CircleCast(_se_c_position, 0.5f, Vector2.zero, 0.5f);

            if (_raycast.collider != null)
            {
                if (_raycast.collider.GetComponent<BoxCollider2D>() != null && _raycast.transform.tag == "Map" && _raycast.collider.GetComponent<BoxCollider2D>() != _shootingwall)
                {
                    break;
                }
            }

            if (_shootingwall.OverlapPoint(_se_c_position))
            {
                _shoot_vector = new Vector2((float)Math.Cos(angle_test), (float)Math.Sin(angle_test));
                _shoot_vector *= _shoot_force;
                Shoot(_shoot_vector);
                break;
            }
        }
        yield return null;
    }

    private void alltestshooting(int coroutine_num)
    {
        float base_offset_angle = 180 / coroutine_num;
        _angle = (float)Math.PI / 2f;
        _shoot_force += 1;
        if (_shoot_force >= _maxforce)
        {
            _state = STATE.Moving;
            return;
        }

        for (int i = 0; i <= coroutine_num; i++)
        {
            float offset_angle = 0;
            if (_target.transform.position.x - transform.transform.position.x > 0)
            {
                offset_angle = base_offset_angle * i;
            }
            else offset_angle = base_offset_angle * i + 180;
            StartCoroutine(TestShooting(_angle - (offset_angle * Mathf.Deg2Rad)));
            if (_state != STATE.TestingShooting) break;
        }
    }
    IEnumerator TestShooting(float angle_test)
    {
        _se_oldpos.Clear();
        Vector2 _se_c_position;
        Vector2 _se_c_velocity;
        _se_c_position = RB.position;
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(angle_test), (float)Math.Sin(angle_test));
        _shoot_vector *= _shoot_force;
        _se_c_velocity = _shoot_vector;

        for (int i = 0; i < _se_iteration; i++)
        {
            _se_oldpos.Add(_se_c_position);
            DrawDebugShooting();

            _se_c_position += _se_c_velocity * Time.fixedDeltaTime;
            _se_c_velocity += new Vector2(0, -9.80665f) * Time.fixedDeltaTime;
            _se_c_velocity += gamemanager.GetComponent<GameScript>().wind * Time.fixedDeltaTime;

            var _raycast = Physics2D.CircleCast(_se_c_position, 0.5f, Vector2.zero, 0.5f);

            if (_raycast.collider != null)
            {
                if (_raycast.collider.GetComponent<BoxCollider2D>() != null && _raycast.transform.tag == "Map")
                {
                    break;
                }
            }

            if (_target.GetComponent<CapsuleCollider2D>().OverlapPoint(_se_c_position))
            {
                float angletest = UnityEngine.Random.Range(-_rand_angle, _rand_angle);
                float angle_randomized = angle_test + (angletest * Mathf.Deg2Rad);
                _shoot_vector = new Vector2((float)Math.Cos(angle_randomized), (float)Math.Sin(angle_randomized));
                _shoot_vector *= _shoot_force;
                Shoot(_shoot_vector);
                break;
            }
        }
        yield return null;
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
            if (_walking_timer > _walking_max_timer/2.5f) 
            {
                _walking_timer = _walking_max_timer/2.5f;
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
        _state = STATE.Escaping;
        gamemanager.GetComponent<GameScript>().ActionTimer();
        gamemanager.GetComponent<GameScript>().FollowThis(newball, this.gameObject);
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
            if (HoleLeftBuffer > 5)
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
            if (HoleRightBuffer > 5)
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
