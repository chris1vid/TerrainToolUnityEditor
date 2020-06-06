using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            AddFilter();
        }
    }

    void AddFilter()
    {
    }
}

 


