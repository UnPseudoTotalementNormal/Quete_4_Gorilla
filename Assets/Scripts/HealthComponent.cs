using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public int health = 3;

    public void ReduceHp(int hp)
    {
        health -= hp;
        if (health <= 0)
        {
            Destroy(GetComponentInParent<Transform>().gameObject);
        }
    }
}
