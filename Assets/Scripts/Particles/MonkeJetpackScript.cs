using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonkeJetpackScript : MonoBehaviour
{
    private Transform _monkesprite;
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _monkesprite = transform.parent.GetComponent<Transform>();
        _spriteRenderer = _monkesprite.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_spriteRenderer.flipX)
        {
            transform.position = _monkesprite.Find("FirePartRightPos").position;
        }
        else
        {
            transform.position = _monkesprite.Find("FirePartLeftPos").position;
        }
    }
}
