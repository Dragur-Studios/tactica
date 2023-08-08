using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexWorldGenerator))]
public class HexWorldGeneratorEditor : Editor
{
    HexWorldGenerator layout;

    bool all_toggle = false;
    bool collision = false;
    bool ui_anchor = false;
    bool meshes = true;
    bool waypoints = true;

    bool algorithmSettingsFoldoutOpen = true;
    bool generationIncludeSetttingsFoldoutOpen = false;

    Vector2Int offset = Vector2Int.zero;

    void OnEnable()
    {   
        layout = (target as HexWorldGenerator);
    }

    HashSet<KeyValuePair<Vector2Int, float>> heightmap = new HashSet<KeyValuePair<Vector2Int, float>>();

    public override void OnInspectorGUI()
    {
        /// TODO! convert to serialized object update so that it updates the actual generator script
        base.OnInspectorGUI();

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Generation Algorithm", EditorStyles.boldLabel);
        layout.algorithm = (GeoGenAlgorithm)EditorGUILayout.EnumPopup((System.Enum)layout.algorithm);
        EditorGUILayout.Space(20);

        if (layout.algorithm == GeoGenAlgorithm.Perlin)
        {
            algorithmSettingsFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(algorithmSettingsFoldoutOpen, "Settings");
            if (algorithmSettingsFoldoutOpen)
            {
                GUILayout.BeginHorizontal();
                layout.seed = EditorGUILayout.IntField(new GUIContent("Seed"), layout.seed);
                if (GUILayout.Button("Random"))
                {
                    System.Random prng = new System.Random((int)Time.time);
                    layout.seed = prng.Next(-10000, 10000);
                }
                GUILayout.EndHorizontal();

                layout.offset = EditorGUILayout.Vector2IntField(new GUIContent("Height Multiplier"), layout.offset);
                layout.maxHeight = EditorGUILayout.Slider(new GUIContent("Height Multiplier"), layout.maxHeight, HexWorldGenerator.minHeightMultiplier, HexWorldGenerator.maxHeightMultiplier);
                layout.heightMultiplierCurve = EditorGUILayout.CurveField(new GUIContent("Height Curve"), layout.heightMultiplierCurve);
                GUILayout.Space(10.0f);
                layout.lacunarity = EditorGUILayout.Slider(new GUIContent("Lacunarity"), layout.lacunarity, HexWorldGenerator.minLacunarity, HexWorldGenerator.maxLacunarity);
                layout.persistance = EditorGUILayout.Slider(new GUIContent("Persistance"), layout.persistance, HexWorldGenerator.minPersistance, HexWorldGenerator.maxPersitance);
                layout.scale = EditorGUILayout.Slider(new GUIContent("Scale"), layout.scale, HexWorldGenerator.minScale, HexWorldGenerator.maxScale);
                layout.octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves"), layout.octaves, HexWorldGenerator.minOctaves, HexWorldGenerator.maxOctaves);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        
        }
        else if (layout.algorithm == GeoGenAlgorithm.DiamondSquare)
        {
            algorithmSettingsFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(algorithmSettingsFoldoutOpen, "Settings");
            EditorGUILayout.HelpBox("WAIT!!!! DO NOT USE UNTIL REMOVED!!", MessageType.Warning);
            if (algorithmSettingsFoldoutOpen)
            {
                GUILayout.BeginHorizontal();
                layout.seed = EditorGUILayout.IntField(new GUIContent("Seed"), layout.seed);
                if (GUILayout.Button("Random"))
                {
                    System.Random prng = new System.Random((int)Time.time);
                    layout.seed = prng.Next(-10000, 10000);
                }
                GUILayout.EndHorizontal();

                layout.mapSize = EditorGUILayout.IntSlider(new GUIContent("Map Size"), layout.mapSize, 1, 128);
                layout.roughness = EditorGUILayout.Slider(new GUIContent("Roughness"), layout.roughness, 0, 10);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        EditorGUILayout.Space(20);


        generationIncludeSetttingsFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(generationIncludeSetttingsFoldoutOpen, "Include", EditorStyles.foldoutHeader);
        if (generationIncludeSetttingsFoldoutOpen)
        {
            meshes = EditorGUILayout.Toggle("Meshes", meshes);
            waypoints = EditorGUILayout.Toggle("Waypoints", waypoints);
            collision = EditorGUILayout.Toggle("Collisions", collision);
            ui_anchor = EditorGUILayout.Toggle("UI", ui_anchor);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate World"))
        {
            heightmap.Clear();
            if (layout.algorithm == GeoGenAlgorithm.Perlin)
            {
                heightmap = HeightMapGenerator.Perlin(layout, layout.seed, layout.offset, layout.scale, layout.octaves, layout.persistance, layout.lacunarity);
                layout.GenerateGrid(heightmap, meshes, waypoints, collision, ui_anchor);

            }else if(layout.algorithm == GeoGenAlgorithm.DiamondSquare)
            {
                //heightmap = HeightMapGenerator.DiamondSquare(layout, layout.seed, layout.cell_count, layout.roughness);
                //layout.GenerateGrid(heightmap, meshes, waypoints, collision, ui_anchor);

            }
        }

        if (GUILayout.Button("Clear"))
        {
            layout.Clear();
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("World Name");
        if(string.IsNullOrEmpty(layout.worldName))
        {
            layout.worldName = "DefaultWorld";
        }

        layout.worldName = EditorGUILayout.TextField(layout.worldName);

        if (GUILayout.Button("Bake"))
        {
            if (layout.heightmap == null)
                return;

            layout.Bake(layout.worldName);
        }

        GUILayout.EndHorizontal();


    }

    

}
