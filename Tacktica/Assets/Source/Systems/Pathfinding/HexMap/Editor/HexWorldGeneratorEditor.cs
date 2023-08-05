using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexWorldGenerator))]
public class HexWorldGeneratorEditor : Editor
{
    HexWorldGenerator layout;
    HexPathfinder pathfinder;

    bool all_toggle = false;
    bool collision = false;
    bool ui_anchor = false;
    bool meshes = true;
    bool waypoints = true;

    bool isOpen = true;
    Vector2Int offset = Vector2Int.zero;

    void OnEnable()
    {   
        layout = (target as HexWorldGenerator);
        pathfinder = layout.GetComponent<HexPathfinder>();
    }

    Dictionary<Vector2Int, float> heightmap = new Dictionary<Vector2Int, float>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Include");
        EditorGUILayout.Separator();

        meshes = EditorGUILayout.Toggle("Meshes", meshes );
        waypoints = EditorGUILayout.Toggle("Waypoints", waypoints );
        collision = EditorGUILayout.Toggle("Collisions", collision );
        ui_anchor = EditorGUILayout.Toggle("UI", ui_anchor );

        EditorGUILayout.Space(20);
        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();
        layout.seed = EditorGUILayout.IntField(new GUIContent("Seed"), layout.seed);
        if (GUILayout.Button("Random"))
        {
            System.Random prng = new System.Random((int)Time.time);
            layout.seed = prng.Next(-10000, 10000);
        }
        GUILayout.EndHorizontal();

        layout.algorithm = (GeoGenAlgorithm)EditorGUILayout.EnumPopup((System.Enum)layout.algorithm);


        if (layout.algorithm == GeoGenAlgorithm.Perlin)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "Settings");
            if (isOpen)
            {
                layout.maxHeight = EditorGUILayout.Slider(new GUIContent("Height Multiplier"), layout.maxHeight, HexWorldGenerator.minHeightMultiplier, HexWorldGenerator.maxHeightMultiplier);
                layout.heightMultiplierCurve = EditorGUILayout.CurveField(new GUIContent("Height Curve"), layout.heightMultiplierCurve);
                GUILayout.Space(10.0f);
                layout.lacunarity = EditorGUILayout.Slider(new GUIContent("Lacunarity"), layout.lacunarity, HexWorldGenerator.minLacunarity, HexWorldGenerator.maxLacunarity);
                layout.persistance = EditorGUILayout.Slider(new GUIContent("Persistance"), layout.persistance, HexWorldGenerator.minPersistance, HexWorldGenerator.maxPersitance);
                layout.scale = EditorGUILayout.Slider(new GUIContent("Scale"), layout.scale, HexWorldGenerator.minScale, HexWorldGenerator.maxScale);
                layout.octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves"), layout.octaves, HexWorldGenerator.minOctaves, HexWorldGenerator.maxOctaves);

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        else if (layout.algorithm == GeoGenAlgorithm.DiamondSquare)
        {
            isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(isOpen, "Settings");
            EditorGUILayout.HelpBox("WAIT!!!! DO NOT USE UNTIL REMOVED!!", MessageType.Warning);
            if (isOpen)
            {
                layout.mapSize = EditorGUILayout.IntSlider(new GUIContent("Map Size"), layout.mapSize, 1, 128);
                layout.roughness = EditorGUILayout.Slider(new GUIContent("Roughness"), layout.roughness, 0, 10);

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }


        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate World"))
        {
            heightmap.Clear();
            if (layout.algorithm == GeoGenAlgorithm.Perlin)
            {
                heightmap = HeightMapGenerator.Perlin(layout, layout.seed, layout.maxHeight, layout.scale, layout.octaves, layout.persistance, layout.lacunarity);
                layout.GenerateGrid(heightmap, meshes, waypoints, collision, ui_anchor);

            }else if(layout.algorithm == GeoGenAlgorithm.DiamondSquare)
            {
                heightmap = HeightMapGenerator.DiamondSquare(layout, layout.seed, layout.cell_count, layout.roughness);
                layout.GenerateGrid(heightmap, meshes, waypoints, collision, ui_anchor);

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
