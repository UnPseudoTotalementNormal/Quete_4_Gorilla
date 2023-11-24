using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private Rigidbody2D RB;
    private Vector2 _awakevel;

    private int min_height = -13;
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
        if (RB.position.y < min_height)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetAngle(Vector2 angle_vector, float force)
    {
        _awakevel = angle_vector * force;
        RB.velocity = _awakevel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Map")
        {
            if (collision.GetComponent<Rigidbody2D>() == null) 
            {
                collision.AddComponent<Rigidbody2D>();
                PhysicsMaterial2D mapmaterial = new PhysicsMaterial2D();
                mapmaterial.bounciness = .5f;
                collision.GetComponent<Rigidbody2D>().sharedMaterial = mapmaterial;
            }
            Rigidbody2D maprb = collision.GetComponent<Rigidbody2D>();
            Vector2 dist = collision.GetComponent<Transform>().position - GetComponent<Transform>().position;
            maprb.velocity = dist.normalized * 10;
            maprb.angularVelocity = Random.Range(-200, 200);
        }
        Destroy(this.gameObject);
    }
}
