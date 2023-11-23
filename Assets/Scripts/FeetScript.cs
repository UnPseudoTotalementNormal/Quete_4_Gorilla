using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetScript : MonoBehaviour
{
    // Start is called before the first frame update
    private player p;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        p = GetComponentInParent<player>();
        p.FeetTouched(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p = GetComponentInParent<player>();
        p.FeetTouched(collision, false);
    }
}
