using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxleFarm : MonoBehaviour
{
    public GameObject groundBlock;
    public float amplitude = 5f;
    public float freq = 8f;
    public int cols = 75;
    public int rows = 75;

    void Start()
    {
        generateTerrain();
    }

    void generateTerrain()
    {
        for(int x = 0; x<cols; x++)
        {
            for(int z = 0; z<rows; z++)
            {
                float y = Mathf.PerlinNoise(x/freq, z/freq) * amplitude;

                GameObject newBlock = GameObject.Instantiate(groundBlock);
                newBlock.transform.position = new Vector3(x, y, z);
            }
        }
    }
}
