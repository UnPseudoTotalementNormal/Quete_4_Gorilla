using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public int health = 3;
    [SerializeField] private GameObject damage_particle;

    private MoneySource money_source;

    public int magic_shield = 0;
    public int magic_shield_max = 0;

    private void Start()
    {
        money_source = GetComponentInParent<MoneySource>();
    }

    public void ReduceHp(int hp)
    {
        if (magic_shield > 0)
        {
            magic_shield -= 1;
        }
        else
        {
            health -= hp;
            if (damage_particle != null)
            {
                GameObject newpart = Instantiate(damage_particle, transform.position, Quaternion.identity);
                newpart.GetComponent<ParticleSystem>().Play();
                Destroy(newpart.gameObject, 10);
            }
            if (money_source != null)
            {
                money_source.GiveMoney();
            }
        }
        
        if (health <= 0)
        {
            Destroy(GetComponentInParent<Transform>().gameObject);
        }
    }

    public void GiveHp(int hp)
    {
        health += hp;
    }
    
    public void GiveMagicShield(int shield)
    {
        magic_shield = Mathf.Clamp(magic_shield + shield, 0, magic_shield_max);
    }
}
