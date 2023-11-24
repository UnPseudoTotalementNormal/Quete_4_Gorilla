using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class EnnemiScript : MonoBehaviour
{
    private enum STATE
    {
        Idle,
        TestingShooting,
    }

    [SerializeField] private GameObject balls;

    private Rigidbody2D RB;
    private int _state = (int)STATE.Idle;

    private float _shoot_timer = 0;
    private int _shoot_max_timer = 5; //in seconds

    private float _angle = 0;
    private float _shoot_force;

    private List<Vector2> _se_oldpos = new List<Vector2>(); //se == shoot emulate
    private Vector2 _se_position;
    private Vector2 _se_velocity;

    private GameObject _target;

    private int _se_iteration = 400;

    private int _min_height = -13;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        _target = GameObject.Find("Player");
    }
    void Start()
    {

    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        switch (_state)
        {
            case (int)STATE.Idle:
                WaitShooting();
                break;

            case (int)STATE.TestingShooting:
                for (int i = 0; i < 200; i++)
                {
                    TestShooting();
                    if (_state != (int)STATE.TestingShooting) break;
                }
                break;
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
    private void TestShooting()
    {
        DrawDebugShooting();
        _angle += 0.0015f;
        if (_angle > 3 * Math.PI/2)
        {
            _angle = (float)Math.PI/2.2f;
            _shoot_force += 1;
        }

        _se_position = RB.position;
        _se_oldpos.Clear();
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        _shoot_vector *= _shoot_force;
        _se_velocity = _shoot_vector;
        
        for (int i = 0; i < _se_iteration; i++)
        {
            if (_se_position.y < _min_height)
            {
                break;
            }

            _se_oldpos.Add(_se_position);

            _se_position += _se_velocity * Time.fixedDeltaTime;
            _se_velocity += new Vector2(0, -9.80665f) * Time.fixedDeltaTime;

            var _raycast = Physics2D.CircleCast(_se_position, 0.5f, _se_velocity.normalized, 0.5f);

            if (_raycast.collider != null)
            {
                if (_raycast.collider.GetComponent<BoxCollider2D>() != null) 
                {
                    break;
                }
            }

            if (_target.GetComponent<CapsuleCollider2D>().OverlapPoint(_se_position))
            {
                Shoot(_shoot_vector);
                _state = (int)STATE.Idle;
                break;
            }
        }
    }
    private void StartTestShooting()
    {
        _angle = (float)Math.PI / 2;
        _shoot_timer = 0;
        _shoot_force = 5;
        _state = (int)STATE.TestingShooting;
        
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
    }
}
