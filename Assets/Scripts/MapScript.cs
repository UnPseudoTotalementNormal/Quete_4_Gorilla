using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    private Vector2 _spawn_pos = new Vector2(-25.15f, -12.17f);
    private Vector2 _min_scale = new Vector2(1.5f, 2);
    private Vector2 _max_scale = new Vector2(4, 17);
    private int n_building = 20;

    [SerializeField] GameObject _square; 
    void Start()
    {
        for (int i = 0; i < n_building; i++) {
            var building = Instantiate(_square, GameObject.Find("Map").transform);
            building.transform.localScale = new Vector3(Random.Range(_min_scale.x, _max_scale.x), Random.Range(_min_scale.y, _max_scale.y), 1);
            building.transform.position = _spawn_pos + new Vector2(building.transform.localScale.x / 2, 0);
            building.GetComponent<SpriteRenderer>().color = Color.grey;
            _spawn_pos += new Vector2(building.transform.localScale.x, 0);

        }
    }
    void Update()
    {
        
    }
}
