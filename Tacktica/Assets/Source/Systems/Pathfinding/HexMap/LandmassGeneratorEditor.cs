using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGridLandmassGenerator))]
public class LandmassGeneratorEditor : Editor
{
    HexGridLandmassGenerator generator;

    int seed = 0;
    GeoGenAlgorithm algorithm = GeoGenAlgorithm.Perlin;

    bool isOpen = true;
    
    // ds settings
    public int mapSize = 64; // Make sure it's 2^n + 1 for the Diamond-Square algorithm
    public float roughness = 0.5f;
    
    // perlin settings
    public float frequency = 0.1f;
    public float noiseScale = 0.17f;
    public int octaves = 4;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(generator == null)
            generator = (HexGridLandmassGenerator)target;

        GUILayout.BeginHorizontal();
        seed = EditorGUILayout.IntField(new GUIContent("Seed"), seed);
        if (GUILayout.Button("Random"))
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        GUILayout.EndHorizontal();

        if(algorithm == GeoGenAlgorithm.Perlin)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "Perlin Settings");
            if (isOpen)
            {
                frequency = EditorGUILayout.Slider(new GUIContent("Frequency"), frequency, 0.01f, 1.0f);
                noiseScale = EditorGUILayout.Slider(new GUIContent("Noise Scale"), noiseScale, 0.01f, 1.0f);
                octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves"), octaves, 1, 12);
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else if(algorithm == GeoGenAlgorithm.DiamondSquare)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "DiamondSquare Settings");
            if (isOpen)
            {
                mapSize = EditorGUILayout.IntSlider(new GUIContent("Map Size"), mapSize, 1, 128);
                roughness = EditorGUILayout.Slider(new GUIContent("Roughness"), roughness, 0, 1);

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        GUILayout.BeginHorizontal();
        algorithm = (GeoGenAlgorithm)EditorGUILayout.EnumPopup((System.Enum)algorithm);

        if (GUILayout.Button("Add Noise"))
        {
            //generator.Generate(seed, algorithm);
            if (algorithm == GeoGenAlgorithm.Perlin)
            {
                generator.GeneratePerlin(seed, noiseScale, frequency, octaves);

            }
            else if (algorithm == GeoGenAlgorithm.DiamondSquare)
            {
               generator.GenerateAsDiamondSquare(seed, mapSize, roughness);
            }

        }
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Flatten"))
        {
            generator.FlattenWorld();
        }
    }
}
  








