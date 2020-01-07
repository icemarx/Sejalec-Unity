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
		
		//TODO: odloci se kje tocno bojo predmeti lahko postavljeni, da niso preblizu
		
		//izberi random block v notranjosti (x in z v obmocju 25-35)
		float xTree = (float) rand.Next(25,36);
		float zTree = (float) rand.Next(25,36);
		float yTree = 2f;
		
		//izberi random block v zunanjem obrocu (x in z v obmocju 13-23 in 46-56)
		float xWell = (float) rand.Next(22);
		if (xWell < 11) xWell += 13;
		else xWell = (xWell % 11) + 46;
		float zWell = (float) rand.Next(22);
		if (zWell < 11) zWell += 13;
		else zWell += (xWell % 11) + 46;
		float yWell = 2f;
		
		//preblizu sejalcu postavi ga malce v stran
		if (xWell < 52f && xWell > 48f && zWell < 52f && zWell > 48f) {
			xWell = 45f;
			zWell = 55f;
		}
		//izberi block na hribcku ob igralcu
		float xKozolec = 43f;
		float zKozolec = 41f;
		float yKozolec = 2f; 

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
				if (xKozolec == x && zKozolec == z) yKozolec = y;
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
