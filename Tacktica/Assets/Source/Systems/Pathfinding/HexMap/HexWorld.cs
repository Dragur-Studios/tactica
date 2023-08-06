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
            
            node.cell.center = new GameObject("center").transform;
            
            AddAnchor(node);
            
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

    private static void AddAnchor(HexNode node)
    {
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
