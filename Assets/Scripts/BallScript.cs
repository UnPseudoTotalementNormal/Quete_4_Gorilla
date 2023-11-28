using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionRadius;

    private Rigidbody2D RB;
    private Vector2 _awakevel;

    private int min_height = -30;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
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
        if (ExplosionRadius != null)
        {
            ExplosionRadius.GetComponent<Rigidbody2D>().excludeLayers = RB.excludeLayers;
            GameObject newexplosion = null;
            newexplosion = Instantiate(ExplosionRadius);
            newexplosion.transform.position = transform.position;
            ExplosionRadius = null;
        }
        Destroy(this.gameObject);
    }
}
