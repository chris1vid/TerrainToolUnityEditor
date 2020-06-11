using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
        public int overlap;
    }

    public SplatHeights[] splatHeights;


    void normalize(float[] v) // accepts a float array -> in the case of it being splat...
    {
        float t = 0;                        //set float to 0 t
        for (int i = 0; i < v.Length; i++) //for times equal to v.length (how many items there are in splat[])
        {
            t = t + v[i];                      //add splat[i] to t
        }

        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= t;                      // after that divide splash[i] by the total...
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y=0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);

                float[] splat = new float[splatHeights.Length];

                for (int i = 0; i < splatHeights.Length; i++)
                {

                    float thisHeightStart = splatHeights[i].startingHeight * Mathf.PerlinNoise(x,y) - splatHeights[i].overlap * Mathf.PerlinNoise(x, y);

                    float nextHeightStart = 0;

                    if (i != splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startingHeight * Mathf.PerlinNoise(x, y) + splatHeights[i + 1].overlap * Mathf.PerlinNoise(x, y);
                    }

                    if (i == splatHeights.Length - 1 && terrainHeight >= splatHeights[i].startingHeight)
                    {
                        splat[i] = 1;
                    }

                    else if (terrainHeight >= splatHeights[i].startingHeight && terrainHeight <= splatHeights[i + 1].startingHeight)
                    {
                        splat[i] = 1;
                    }
                    
                    for(int j=0; j < splatHeights.Length; j++)
                    {
                        splatmapData[x, y, j] = splat[j];
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }

    
}
