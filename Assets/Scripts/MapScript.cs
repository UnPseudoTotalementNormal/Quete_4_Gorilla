using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    private Vector2 _spawn_pos = new Vector2(-10.65f, -3.6f);
    private Vector2 _min_scale = new Vector2(1.5f, 2);
    private Vector2 _max_scale = new Vector2(4, 17);

    [SerializeField] GameObject _square; 
    void Start()
    {
        for (int i = 0; i < 20; i++) {
            var immeuble = Instantiate(_square);
            immeuble.transform.localScale = new Vector3(Random.Range(_min_scale.x, _max_scale.x), Random.Range(_min_scale.y, _max_scale.y), 1);
            immeuble.transform.position = _spawn_pos + new Vector2(immeuble.transform.localScale.x / 2, 0);
            immeuble.GetComponent<SpriteRenderer>().color = Color.grey;
            _spawn_pos += new Vector2(immeuble.transform.localScale.x, 0);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
