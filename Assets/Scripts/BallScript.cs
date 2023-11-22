using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D RB;
    private Vector2 _awakevel;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }



    public void SetAngle(Vector2 angle_vector)
    {
        angle_vector = angle_vector.normalized;
        _awakevel = angle_vector * 10;
        RB.velocity = _awakevel;
    }
}
