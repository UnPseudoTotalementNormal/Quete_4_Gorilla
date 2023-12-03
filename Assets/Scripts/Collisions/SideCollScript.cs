using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollScript : MonoBehaviour
{
    [SerializeField] private bool left;
    private EnnemiScript e;

    private void OnTriggerStay2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        if (left)
        {
            e.LeftSideTouched(collision, true);
        }
        else
        {
            e.RightSideTouched(collision, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        if (left)
        {
            e.LeftSideTouched(collision, false);
        }
        else
        {
            e.RightSideTouched(collision, false);
        }
    }
}
