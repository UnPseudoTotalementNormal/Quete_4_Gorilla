using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionRadiusScript : MonoBehaviour
{
    private List<Collider2D> oldcolliders = new List<Collider2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
        foreach (Collider2D icollider in oldcolliders) 
        {
            if (icollider == collision)
            {
                return;
            }
        }
        oldcolliders.Add(collision);
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
            maprb.velocity = -dist.normalized * 15;
            maprb.angularVelocity = Random.Range(-200, 200);
        }
        else
        {
            Debug.Log(collision);
            GameObject newobject = new GameObject();
            newobject.AddComponent<AudioPlayer>();
            newobject.GetComponent<AudioPlayer>().PlayAudio("Audio/pop");
        }
    }
}
