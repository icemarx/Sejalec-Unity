using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxleFarm : MonoBehaviour
{
    public GameObject groundBlock;
    public GameObject borderBlock;

    public float amplitude = 5f;
    public float freq = 8f;
    public int cols = 75;
    public int rows = 75;

    public int borderSize = 4;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        System.Random rand = new System.Random();

        for (int x = 0; x<cols; x++)
        {
            for(int z = 0; z<rows; z++)
            {
                GameObject newBlock;

                float y = Mathf.PerlinNoise(x/freq, z/freq) * amplitude;

                if(rand.NextDouble() < BorderWeight(x, z))
                {
                    newBlock = GameObject.Instantiate(borderBlock);
                }
                else
                {
                    newBlock = GameObject.Instantiate(groundBlock);
                }

                newBlock.transform.position = new Vector3(x, y, z);
            }
        }
    }

    double BorderWeight(int x, int z)
    {
        int offsetX = OffsetByCoordinate(x, cols);
        int offsetZ = OffsetByCoordinate(z, rows);

        int offset = offsetX;

        if(offsetZ < offset)
        {
            offset = offsetZ;
        }

        if(offset == 0)
        {
            return 1;
        }
        else if(offset < borderSize)
        {
            return 1 - Math.Log(offset+1) / Math.Log(borderSize);
        } 
        else
        {
            return 0;
        }

    }

    int OffsetByCoordinate(int coordinate, int dimensionSize)
    {
        int center = dimensionSize / 2;
        if(coordinate<center)
        {
            return coordinate;
        } else
        {
            return dimensionSize - 1 - coordinate;
        }
    }
}
