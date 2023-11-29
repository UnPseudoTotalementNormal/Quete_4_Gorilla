using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionRadius;
    private GameScript gamemanager;

    private Rigidbody2D RB;
    private Vector2 _awakevel;

    private int min_height = -30;
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
    }
    private void FixedUpdate()
    {
        RB.velocity += gamemanager.wind * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(transform.position.y + RB.velocity.y - transform.position.y, transform.position.x + RB.velocity.x - transform.position.x) * Mathf.Rad2Deg);
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
