using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxleFarm : MonoBehaviour
{
    public GameObject groundBlock;
    public GameObject borderBlock;
	//public GameObject treeOfLife;
	//public GameObject well;
	//public GameObject kowzowlec;

    public float amplitude = 5f;
    public float freq = 8f;
    public int cols = 75;
    public int rows = 75;

    public int borderSize = 4;

    private int _groundBlocksCount = 0;

    void Awake()
    {
        GenerateTerrain();
    }

    public int GetGroundBlocksCount()
    {
        return _groundBlocksCount;
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
                    _groundBlocksCount++;
                }

                newBlock.transform.position = new Vector3(x, y, z);
                newBlock.transform.parent = transform;
            }
        }
		
		GenerateTree();
		GenerateWell();
		GenerateKowzowlec();
    }
	
	void GenerateTree() {
		//izberi random block v notranjosti (x in z v obmocju 30-45)
		//
		//GameObject newTree = GameObject.Instantiate(treeOfLife);
		//newTree.transform.position = new Vector3(randomBlock.position.x,randomBlock.position.y,randomBlock.position.z);
		//newTree.transform.parent = transform; karkoli je ze to
	}
	
	void GenerateWell() {
		//izberi random block v zunanjem obrocu (x in z v obmocju 10-20 in 55-65)
		//
		//GameObject newWell = GameObject.Instantiate(well);
		//newWell.transform.position = new Vector3(randomBlock.position.x,randomBlock.position.y,randomBlock.position.z);
		//newWell.transform.parent = transform; karkoli je ze to
	}
	
	void GenerateKowzowlec() {
		//izberi block tik ob sejalcu na zacetku
		//
		//GameObject newKowzowlec = GameObject.Instantiate(kowzowlec);
		//newKowzowlec.transform.position = new Vector3(block.position.x,block.position.y,block.position.z);
		//newKowzowlec.transform.parent = transform; karkoli je ze to
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
