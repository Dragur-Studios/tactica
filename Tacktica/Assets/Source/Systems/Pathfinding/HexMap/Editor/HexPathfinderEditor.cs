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
    HexagonGrid layout;

    Vector2Int start;
    Vector2Int end;
    bool hasPath = false;
    List<Vector2Int> path = new List<Vector2Int>();

    private void OnEnable()
    {
        pathfinder = (HexPathfinder)target;
        layout = pathfinder.GetComponent<HexagonGrid>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        start = EditorGUILayout.Vector2IntField("Start Node", start);
        if (GUILayout.Button("Random"))
        {
            start = new Vector2Int(Random.Range(0, layout.gridResolution + 1), Random.Range(0, layout.gridResolution));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        end = EditorGUILayout.Vector2IntField("End Node", end);
        if (GUILayout.Button("Random"))
        {
            do
            {
                end = new Vector2Int(Random.Range(0, layout.gridResolution + 1), Random.Range(0, layout.gridResolution));
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

