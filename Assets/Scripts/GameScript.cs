using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private GameObject turn;

    private Camera cam;
    private float camZ;

    private void Start()
    {
        cam = Camera.main;
        camZ = cam.transform.position.z;
        turn = GameObject.Find("Player1");
    }

    private void FixedUpdate()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, turn.transform.position, 3f * Time.fixedDeltaTime);
        cam.transform.position += new Vector3(0, 0, -cam.transform.position.z + camZ);
    }
}
