using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    }
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (RB.position.y < -4)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetAngle(Vector2 angle_vector, float force)
    {
        _awakevel = angle_vector * force;
        RB.velocity = _awakevel;
    }
}
