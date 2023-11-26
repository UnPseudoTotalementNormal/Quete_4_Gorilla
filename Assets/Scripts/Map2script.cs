using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map2script : MonoBehaviour
{
    [SerializeField] int startX, startY;
    [SerializeField] int width, height;
    [SerializeField] int minHeight, maxHeight;
    [SerializeField] int maxVariation;
    [SerializeField] int repeatNum;
    [SerializeField] GameObject Dirt, Grass;
    void Start()
    {
        PreGeneration();
        Generation();
        PostGeneration();
    }

    void PreGeneration()
    {
        int x = startX - 16;
        height = maxHeight + 30;
        while (x < startX) 
        {
            GenerateFlatPlatform(x);
            x++;
        }
    }

    void PostGeneration()
    {
        int x = width;
        height = maxHeight + 30;
        while (x < width + 16)
        {
            GenerateFlatPlatform(x);
            x++;
        }
    }
    void Generation()
    {
        int repeatValue = 0;
        height = Random.Range(minHeight, maxHeight);
        for (int x = startX; x < width; x++)
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
        for (int y = startY; y < height; y++)
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