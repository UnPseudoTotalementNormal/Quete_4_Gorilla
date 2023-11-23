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
    private int _shoot_max_timer = 2; //in seconds

    private float _angle = 0;
    private float _shoot_force;

    private List<Vector2> _se_oldpos = new List<Vector2>(); //se == shoot emulate
    private Vector2 _se_position;
    private Vector2 _se_velocity;

    private GameObject _target;

    private int _se_iteration = 300;
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
                TestShooting();
                break;
        }
    }
    private void WaitShooting()
    {
        _shoot_timer += Time.deltaTime;
        if (_shoot_timer > _shoot_max_timer ) 
        {
            StartTestShooting();
        }
    }
    private void TestShooting()
    {
        DrawDebugShooting();
        _angle += 0.05f;
        if (_angle > 3 * Math.PI/2)
        {
            _angle = (float)Math.PI/2;
            _shoot_force += 1;
        }

        _se_position = RB.position;
        _se_oldpos.Clear();
        Vector2 _shoot_vector = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle));
        _shoot_vector *= _shoot_force;
        _se_velocity = _shoot_vector;

        for (int i = 0; i < _se_iteration; i++)
        {
            if (_se_position.y > -4)
            {
                _se_oldpos.Add(_se_position);
            }
            _se_position += _se_velocity * Time.fixedDeltaTime;
            _se_velocity += new Vector2(0, -9.81f) * Time.fixedDeltaTime;
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
        _shoot_force = 1;
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
        GameObject newball = Instantiate(balls, transform.position + new Vector3(-1, 0, 0), transform.rotation);
        newball.GetComponent<BallScript>().SetAngle(shootvector.normalized, shootvector.magnitude);
    }
}
