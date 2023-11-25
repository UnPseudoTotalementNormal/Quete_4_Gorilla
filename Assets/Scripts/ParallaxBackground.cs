using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector2 cloudoffset = Vector2.zero;
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
            float dist = Camera.main.transform.position.x;
            if (background.tag == "Cloud")
            {
                cloudoffset += ((Vector2.right * 2) / background.transform.position.z) * Time.deltaTime;
                background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Vector2.right * dist / background.transform.position.z + cloudoffset);
                continue;
            } 
            background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Vector2.right * dist / background.transform.position.z);
        }
    }
}
