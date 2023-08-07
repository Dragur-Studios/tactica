using System;
using System.Collections.Generic;
using UnityEngine;


public class HexWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, HexNode> graph = new Dictionary<Vector2Int, HexNode> ();
    public HexWorldData worldData;
    
    private void Start()
    {
        if (worldData == null)
        {
            Debug.Log("<color=red>Error! World Data is Null!</color>");
            return;
        }

        BuildGraph();
    }
   
    void BuildGraph()
    {
        var coords = new List<Vector2Int>();

        worldData.nodes.ForEach(node => coords.Add(node.position));

        for (int i = 0; i < coords.Count; i++)
        {
            var node = new HexNode();
            var data = worldData.nodes[i];
            

            node.cell = HexGrid.CreateCell(transform, HexWorldData.MaxHeight * worldData.enviormentCurve.Evaluate(data.height), coords[i], data.type, worldData.isFlatTopped);
            node.cell.gameObject.isStatic = true;
            
            AddCenter(node);
            AddUI(node);

            graph.Add(coords[i], node);
        }

        for (int i = 0; i < coords.Count; i++)
        {
            var node = graph[coords[i]];

            var neighbors = worldData.nodes[i].neighbors;

            for (int j = 0; j < neighbors.Count; j++)
            {
                var n = graph[neighbors[j]];
                if (n != null)
                {
                    node.neighbors.Add(n);
                }
            }

        }




    }

    private void AddUI(HexNode node)
    {
        var go = new GameObject("ui");
        go.transform.position = node.cell.center.position + (Vector3.one * 1e-6f);
        go.transform.SetParent(node.cell.transform, true);


        // add the pathfinding UI object
        var node_display = new GameObject("node-display", typeof(HexRenderer));
        var hex = node_display.GetComponent<HexRenderer>();
        hex.isFlatTopped = worldData.isFlatTopped;
        hex.outerSize = 1.0f;
        hex.innerSize = 0.9f;
        hex.height = 0.1f;
        hex.GenerateMesh();
        node_display.transform.position = go.transform.position;
        node_display.transform.SetParent(go.transform, true);

        
        // add the node position ui object
        // add any other UI objects here



    }

    private static void AddCenter(HexNode node)
    {
        node.cell.center = new GameObject("center").transform;

        // set the position to the cells transform to start alignment
        node.cell.center.position = node.cell.transform.position;

        // align the center object to the top of the cell
        var center = node.cell.center.position;
        
        if (node.cell.TryGetComponent(out HexRenderer hex))
            center.y += (hex.height) + 1e-4f;   // use the height and some small offset to get around z-fighting
        
        node.cell.center.position = center;
        node.cell.center.SetParent(node.cell.transform, true);

    }
}
