using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxleFarm : MonoBehaviour
{
    public GameObject groundBlock;
    public GameObject borderBlock;
	public GameObject treeOfLife;
	public GameObject well;
	public GameObject kozolec;

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
		
		//izberi random block v notranjosti (x in z v obmocju 31-45)
		float xTree = (float) rand.Next(31,46);
		float zTree = (float) rand.Next(31,46);
		float yTree = 1f; //ce kaksna napaka, da je usaj priblizno
		
		//izberi random block v zunanjem obrocu (x in z v obmocju 20-30 in 46-56)
		float xWell = (float) rand.Next(22);
		if (xWell < 11) xWell += 20;
		else xWell += 36;
		float zWell = (float) rand.Next(22);
		if (zWell < 11) zWell += 20;
		else zWell += 36;
		float yWell = 1f; //ce kaksna napaka, da je usaj priblizno
		
		//izberi block tik ob sejalcu na zacetku
		float xKozolec = 25f;
		float zKozolec = 25f;
		float yKozolec = 3f; //zacasne koordinate

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
				
				if (xWell == x && zWell == z) yWell = y;
				if (xTree == x && zTree == z) yTree = y;
            }
        }
		
		GenerateTree(xTree, yTree, zTree);
		GenerateWell(xWell, yWell, zWell);
		GenerateKowzowlec(xKozolec, yKozolec, zKozolec);
    }
	
	void GenerateTree(float x, float y, float z) {
		GameObject newTree = GameObject.Instantiate(treeOfLife);
		newTree.transform.position = new Vector3(x, y + 0.5f, z);		
	}
	
	void GenerateWell(float x, float y, float z) {
		GameObject newWell = GameObject.Instantiate(well);
		newWell.transform.position = new Vector3(x, y + 0.5f, z);
	}
	
	void GenerateKowzowlec(float x, float y, float z) {
		GameObject newKowzowlec = GameObject.Instantiate(kozolec);
		newKowzowlec.transform.position = new Vector3(x, y + 0.5f, z);
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
