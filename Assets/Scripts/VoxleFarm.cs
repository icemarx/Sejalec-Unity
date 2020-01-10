using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxleFarm : MonoBehaviour
{
    public GameObject groundBlock;
    public GameObject borderBlock;
	public GameObject treeOfLife;
	public GameObject treeOfLifeWithFlowers;
	public GameObject well;
	public GameObject kozolec;

    public float amplitude = 5f;
    public float freq = 8f;
    public int cols = 75;
    public int rows = 75;

    public int borderSize = 4;

    private int _groundBlocksCount = 0;
	
	private const int TREE = 0;
	private const int WELL = 1;

	private GameObject treeInstance;
	private GameObject treeFlowersInstance;

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
		
		//pridobi hribcka
		float[] treeCoord = randomHribcek(TREE);
		float[] wellCoord = randomHribcek(WELL);
		
		float xTree = treeCoord[0];
		float zTree = treeCoord[1];
		float yTree = 2f;
		
		float xWell = wellCoord[0];
		float zWell = wellCoord[1];
		float yWell = 2f;
		
		//izberi block na hribcku ob igralcu    x = 41  z = 20
		float xKozolec = 44f;
		float zKozolec = 20f;
		float yKozolec = 1.7f; 

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
		treeInstance = GameObject.Instantiate(treeOfLife);
		treeInstance.transform.position = new Vector3(x, y + 0.5f, z);

		treeFlowersInstance = GameObject.Instantiate(treeOfLifeWithFlowers);
		treeFlowersInstance.SetActive(false);
		treeFlowersInstance.transform.position = new Vector3(x, y + 0.5f, z);
	}

	public void SwitchTree()
	{
		if (treeInstance.activeSelf)
		{
			treeFlowersInstance.SetActive(true);
			treeInstance.SetActive(false);
		}
		else
		{
			treeInstance.SetActive(true);
			treeFlowersInstance.SetActive(false);
		}
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
	
	float[] randomHribcek(int well_tree) {
		// 0 ==> X
		// 1 ==> Z
        System.Random rand = new System.Random();
		int[,] hribcki;
		int choose;
		if (well_tree == TREE) {
			// hribcki drevo
			// x   z
			// 60  56
			// 45  43
			// 38  53
			// 26  51
			// 57  36
			// 24  35
			hribcki = new int[,] {{60,56},{45,43},{38,53},{26,51},{57,36},{24,35}};
			choose = rand.Next(6);
		}
		else {
			// hribkci vodnjak
			// x   z
			// 15  60
			// 20  39
			// 10  44
			// 12  14
			// 27  19
			// 18  24
			// 34  27
			// 66  45
			hribcki = new int[,] {{15,60},{20,39},{10,44},{12,14},{27,19},{18,24},{34,27},{66,45}};
			choose = rand.Next(8);
		}
		float[] result = new float[2];
		result[0] = (float) hribcki[choose,0];
		result[1] = (float) hribcki[choose,1];
		return result;
	}
}
