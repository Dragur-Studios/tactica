using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexPathfinder))]
public class HexPathfinderEditor : Editor
{
    HexPathfinder pathfinder;
    HexWorldGenerator layout;

    Vector2Int start;
    Vector2Int end;
    bool hasPath = false;
    List<Vector3> path = new List<Vector3>();

    private void OnEnable()
    {
        pathfinder = (HexPathfinder)target;
        layout = pathfinder.GetComponent<HexWorldGenerator>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        start = EditorGUILayout.Vector2IntField("Start Node", start);
        if (GUILayout.Button("Random"))
        {
            start = new Vector2Int(Random.Range(-layout.range, layout.range), Random.Range(-layout.range, layout.range));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        end = EditorGUILayout.Vector2IntField("End Node", end);
        if (GUILayout.Button("Random"))
        {
            do
            {
                end = new Vector2Int(Random.Range(-layout.range, layout.range), Random.Range(-layout.range, layout.range));
            }
            while (end == start);
        }
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Find path"))
        {
            path = pathfinder.FindPath(start, end);
            hasPath = path?.Count > 0;
        }

        EditorGUILayout.Toggle($"Path Possible", hasPath);



    }
}

