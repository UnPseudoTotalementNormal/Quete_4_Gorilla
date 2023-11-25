using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Camera.main.transform.position.x - transform.position.x, 0, 0);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform background = transform.GetChild(i);
            if (background.tag == "Cloud")
            {
                background.GetComponent<Renderer>().material.mainTextureOffset += ((Vector2.right * 2) / background.transform.position.z) * Time.deltaTime;
                continue;
            } 
            float dist = Camera.main.transform.position.x;
            background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Vector2.right * dist / background.transform.position.z);
        }
    }
}
