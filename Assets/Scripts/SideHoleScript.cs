using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideHoleScript : MonoBehaviour
{
    [SerializeField] private bool left;
    private EnnemiScript e;
    private bool inhole = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        if (left)
        {
            //e.LeftSideHole(false);
        }
        else
        {
            //e.RightSideHole(false);
        }
        inhole = false;
    }

    private void FixedUpdate()
    {
        e = GetComponentInParent<EnnemiScript>();
        if (left)
        {
            e.LeftSideHole(inhole);
        }
        else
        {
            e.RightSideHole(inhole);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        e = GetComponentInParent<EnnemiScript>();
        if (left)
        {
            //e.LeftSideHole(true);
        }
        else
        {
            //e.RightSideHole(true);
        }
        inhole = true;
    }
}
