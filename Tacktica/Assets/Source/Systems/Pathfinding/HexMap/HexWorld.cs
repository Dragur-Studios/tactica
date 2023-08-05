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

}
