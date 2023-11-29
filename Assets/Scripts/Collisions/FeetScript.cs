using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetScript : MonoBehaviour
{
    private player p;
    private EnnemiScript e;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        p = GetComponentInParent<player>();
        e = GetComponentInParent<EnnemiScript>();
        if (p != null)
        {
            p.FeetTouched(collision, true);
        }
        else
        {
            e.FeetTouched(collision, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p = GetComponentInParent<player>();
        e = GetComponentInParent<EnnemiScript>();
        if (p != null)
        {
            p.FeetTouched(collision, false);
        }
        else
        {
            e.FeetTouched(collision, false);
        }
    }
}
