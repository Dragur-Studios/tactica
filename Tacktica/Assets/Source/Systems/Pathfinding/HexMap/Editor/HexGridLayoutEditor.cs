using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexagonGrid))]
public class HexGridLayoutEditor : Editor
{
    HexagonGrid layout;
    HexPathfinder pathfinder;
    
    [Header("Perlin Settings")]
    public float frequency = 0.1f;
    public float noiseScale = 0.17f;
    public int octaves = 4;


    void OnEnable()
    {   
        layout = (target as HexagonGrid);
        pathfinder = layout.GetComponent<HexPathfinder>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            layout.GenerateGrid();
        }


       

    }

    

}
