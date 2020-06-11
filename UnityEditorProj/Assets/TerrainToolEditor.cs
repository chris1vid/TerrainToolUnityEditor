using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class TerrainToolEditor : EditorWindow
{



    static Terrain[] terrains;
    static string[] options;

    [MenuItem("Window/Terrain Height Filter")]
    static void ShowWindow()
    {
        terrains = new Terrain[5];
        options = new string[5];

        TerrainToolEditor window = (TerrainToolEditor)EditorWindow.GetWindow(typeof(TerrainToolEditor));
        window.minSize = new Vector2(300, 500);
        window.Show();
        terrains = Terrain.activeTerrains;

        int n = 0;



        foreach (Terrain i in terrains)
        {
            string nk = i.ToString();

            options[n] = nk;
            n++;
        }
        n = 0;
    }




    public int index = 0;
    public int index2 = 0;

    private void OnGUI()
    {
        GUILayout.Label("Test");
        index = EditorGUILayout.Popup(index, options);
        index2 = EditorGUILayout.Popup(index2, options);
        if (GUILayout.Button("Accept"))
        {
            
        }
    }

    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
        public int overlap;
    }

    public SplatHeights[] splatHeights;


    void normalize(float[] v)
    {
        float total = 0;
        for (int i = 0; i < v.Length; i++)
        {
            total += v[i];
        }

        for (int i = 0; i < v.Length; i++)
        {
            v[i] /= total;
        }
    }

    public float map(float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }

    // Start is called before the first frame update
    void Start()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);

                float[] splat = new float[splatHeights.Length];

                for (int i = 0; i < splatHeights.Length; i++)
                {
                    float thisNoise = map(Mathf.PerlinNoise(x * 0.03f, y * 0.03f), 0, 1, 0.5f, 1f);
                    float thisHeightStart = splatHeights[i].startingHeight * thisNoise -
                        splatHeights[i].overlap * thisNoise;

                    float nextHeightStart = 0;
                    if (i != splatHeights.Length - 1)
                    {
                        nextHeightStart = splatHeights[i + 1].startingHeight * thisNoise
                            + splatHeights[i + 1].overlap * thisNoise;

                    }

                    if (terrainHeight >= splatHeights[i].startingHeight && terrainHeight <= nextHeightStart)
                    {
                        splat[i] = 1;
                    }
                    

                    for (int j = 0; j < splatHeights.Length; j++)
                    {
                        splatmapData[x, y, j] = splat[j];
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }

}




