using UnityEngine;
using UnityEditor;

public class TerrainTextureEditorWindow : EditorWindow
{

    [MenuItem("Window/TerrainTexture")]

    public static void ShowWindow ()
    {
        GetWindow<TerrainTextureEditorWindow>("TerrainTexture");
    }

    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex; // unused.. make it so that rather then using the default array method, we search by these
        public int startingHeight;
        public int overlap;
    }

    public bool blendByAngle;

    int b = 0;

    public SplatHeights[] splatHeights; // the splatheights, these need to be converted properly.

    SplatHeights findHeightByIndex(float toFind)
    {
        for (var m = 0; m < splatHeights.Length; m++)
        {
            if (b < 4)
            {
                Debug.Log("Tofind " + toFind + "m:" + m + "texindex: " + splatHeights[m].textureIndex);
                b++;
            }
            if (splatHeights[m].textureIndex == toFind)
            {

                return splatHeights[m];
            }
        }
        return splatHeights[0];
    }

    

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
    Terrain ter;
    void OnGUI()
    {

        //Выводим текст
        GUILayout.Label("Procedurally texture terrain, by Christian S.");

        GUILayout.Label("Terrain:");
        ter = (Terrain)EditorGUILayout.ObjectField(ter, typeof(Terrain), true);

        if (ter != null)
        {

            



            blendByAngle = GUILayout.Toggle(blendByAngle, "Blend by Angle");



            // Array stuff goes here


            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("splatHeights");

            EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
            so.ApplyModifiedProperties(); // Remember to apply modified properties


            if (GUILayout.Button("Apply"))
            {

                Debug.Log("Applied");

                

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
                            float thisNoise = Mathf.Clamp(Mathf.PerlinNoise(x * 0.05f, y * 0.05f), 0.5f, 1f);

                            float thisHeightStart = findHeightByIndex(i).startingHeight * thisNoise - findHeightByIndex(i).overlap * thisNoise;

                            float nextHeightStart = 0;

                            if (i != splatHeights.Length - 1)
                            {

                                nextHeightStart = findHeightByIndex(i + 1).startingHeight * thisNoise + findHeightByIndex(i + 1).overlap * thisNoise;
                            }

                            if (i == splatHeights.Length - 1 && terrainHeight >= findHeightByIndex(i).startingHeight)
                            {
                                splat[findHeightByIndex(i).textureIndex] = 1;

                                
                            }

                            else if (terrainHeight >= findHeightByIndex(i).startingHeight && terrainHeight <= findHeightByIndex(i + 1).startingHeight)
                            { 
                                splat[findHeightByIndex(i).textureIndex] = 1;
                            }

                            //normalize(splat);

                            for (int j = 0; j < splatHeights.Length; j++)
                            {
                                splatmapData[x, y, j] = splat[findHeightByIndex(j).textureIndex];
                            }
                        }

                        if (blendByAngle)
                        {
                            // Get the normalized terrain coordinate that
                            // corresponds to the the point.
                            float normX = x * 1.0f / (terrainData.alphamapWidth - 1);
                            float normY = y * 1.0f / (terrainData.alphamapHeight - 1);

                            var angle = terrainData.GetSteepness(normX, normY);

                            // Steepness is given as an angle, 0..90 degrees. Divide
                            // by 90 to get an alpha blending value in the range 0..1.
                            var frac = angle / 180.0;
                            splatmapData[x, y, 0] = (float)frac;
                            splatmapData[x, y, 1] = (float)(1 - frac);

                            float[] dirHi = new float[5];

                            dirHi[0] = terrainData.GetHeight(y, x);
                            dirHi[1] = terrainData.GetHeight(y + 1, x);
                            dirHi[2] = terrainData.GetHeight(y, x + 1);
                            dirHi[3] = terrainData.GetHeight(y - 1, x);
                            dirHi[4] = terrainData.GetHeight(y, x - 1);

                            // convert to a-n...
                        }



                        //Based on height of it and nearby pos, change texture. 
                    }

                    terrainData.SetAlphamaps(0, 0, splatmapData);
                }
            }

        }
    }
    }


