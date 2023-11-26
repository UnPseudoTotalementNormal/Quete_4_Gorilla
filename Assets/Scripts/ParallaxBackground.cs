using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Vector2 cloudoffset = Vector2.zero;

    void Update()
    {
        transform.position += new Vector3(Camera.main.transform.position.x - transform.position.x, Camera.main.transform.position.y - transform.position.y, 0);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform background = transform.GetChild(i);
            float distx = Camera.main.transform.position.x;
            float disty = Camera.main.transform.position.y;
            if (background.tag == "Cloud")
            {
                cloudoffset += ((Vector2.right * 2) / background.transform.position.z) * Time.deltaTime;
                background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", (Vector2.right * distx / background.transform.position.z) + (Vector2.up * disty / background.transform.position.z) + cloudoffset);
                continue;
            } 
            background.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", (Vector2.right * distx / background.transform.position.z) + (Vector2.up * disty / background.transform.position.z));
        }
    }
}
