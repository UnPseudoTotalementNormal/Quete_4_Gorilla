using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2script : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] int minHeight, maxHeight;
    [SerializeField] int maxVariation;
    [SerializeField] int repeatNum;
    [SerializeField] GameObject Dirt, Grass;
    void Start()
    {
        Generation();
    }

    void Generation()
    {
        int repeatValue = 0;
        height = Random.Range(minHeight, maxHeight);
        for (int x = -25; x < width; x++)
        {
            if (repeatValue == 0)
            {
                height = Random.Range(Mathf.Clamp(height - maxVariation, maxHeight, minHeight), Mathf.Clamp(height + maxVariation, maxHeight, minHeight));
                //height = Random.Range(minHeight, maxHeight);
                GenerateFlatPlatform(x);
                repeatValue = repeatNum;
            }
            else
            {
                GenerateFlatPlatform(x);
                repeatValue--;
            }
        }
    }

    void GenerateFlatPlatform(int x)
    {
        for (int y = -15; y < height; y++)
        {
            SpawnObj(Dirt, x, y);
        }
        SpawnObj(Grass, x, height);
    }

    void SpawnObj(GameObject obj, int width, int height)
    {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
        obj.transform.parent = this.transform;
    }
}