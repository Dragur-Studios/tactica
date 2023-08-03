using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGridLandmassGenerator))]
public class LandmassGeneratorEditor : Editor
{
    HexGridLandmassGenerator generator;
    bool isOpen = true;

    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();

        if(generator == null)
            generator = (HexGridLandmassGenerator)target;
        

        GUILayout.BeginHorizontal();
        generator.seed = EditorGUILayout.IntField(new GUIContent("Seed"), generator.seed);
        if (GUILayout.Button("Random"))
        {
            generator.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        generator.algorithm = (GeoGenAlgorithm)EditorGUILayout.EnumPopup((System.Enum)generator.algorithm);

        if (GUILayout.Button("Add Noise"))
        {
            if (generator.algorithm == GeoGenAlgorithm.Perlin)
            {
                generator.GeneratePerlin(generator.seed, generator.noiseScale, generator.amplitude, generator.frequency, generator.octaves);

            }
            else if (generator.algorithm == GeoGenAlgorithm.DiamondSquare)
            {
                generator.GenerateAsDiamondSquare(generator.seed, generator.mapSize, generator.roughness);
            }

        }
        GUILayout.EndHorizontal();

        if (generator.algorithm == GeoGenAlgorithm.Perlin)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "Settings");
            if (isOpen)
            {
                generator.frequency = EditorGUILayout.Slider(new GUIContent("Frequency"), generator.frequency, 0.01f, 1.0f);
                generator.amplitude = EditorGUILayout.Slider(new GUIContent("Amplitude"), generator.amplitude, 0.01f, 5.0f);
                generator.noiseScale = EditorGUILayout.Slider(new GUIContent("Noise Scale"), generator.noiseScale, 0.01f, 1.0f);
                generator.octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves"), generator.octaves, 1, 12);
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else if(generator.algorithm == GeoGenAlgorithm.DiamondSquare)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "Settings");
            EditorGUILayout.HelpBox("WAIT!!!! DO NOT USE UNTIL REMOVED!!", MessageType.Warning);
            if (isOpen)
            {
                generator.mapSize = EditorGUILayout.IntSlider(new GUIContent("Map Size"), generator.mapSize, 1, 128);
                generator.roughness = EditorGUILayout.Slider(new GUIContent("Roughness"), generator.roughness, 0, 1);

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

       

    }
}
  








