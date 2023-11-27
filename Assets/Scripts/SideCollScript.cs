using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollScript : MonoBehaviour
{
    private EnnemiScript e;

    private void OnTriggerStay2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        e.SideTouched(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        e.SideTouched(collision, false);
    }
}
