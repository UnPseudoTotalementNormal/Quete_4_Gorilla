using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonkeJetpackScript : MonoBehaviour
{
    GameObject _monkesprite;
    void Start()
    {
        _monkesprite = GetComponentInParent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _monkesprite.transform.Find("FirePartLeftPos").GetComponent<Transform>().position;
    }
}
