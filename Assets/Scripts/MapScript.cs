using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1; i++) {
            var immeuble = new GameObject("cool");
            Instantiate(immeuble);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
