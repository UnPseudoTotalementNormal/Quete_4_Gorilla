using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public int health = 3;
    [SerializeField] private GameObject damage_particle;

    public void ReduceHp(int hp)
    {
        health -= hp;
        if (health <= 0)
        {
            Destroy(GetComponentInParent<Transform>().gameObject);
        }
        if (damage_particle != null)
        {
            GameObject newpart = Instantiate(damage_particle, transform.position, Quaternion.identity);
            newpart.GetComponent<ParticleSystem>().Play();
            Destroy(newpart.gameObject, 10);
        }
    }
}
