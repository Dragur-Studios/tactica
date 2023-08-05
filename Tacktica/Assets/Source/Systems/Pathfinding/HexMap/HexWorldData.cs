using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct NodeData
{
    public float height;
    public Vector2Int position;
    public List<Vector2Int> neighbors;
    public ElevationSettings type;

}
public class HexWorldData : ScriptableObject
{
    public const float MaxHeight = 10.0f;
    
    public List<NodeData> nodes = new List<NodeData>();
    
    public AnimationCurve enviormentCurve;

    public bool isFlatTopped = false;

    public void Decompose(Dictionary<Vector2Int, HexNode> graph, Dictionary<Vector2Int, float> heightmap, List<ElevationSettings> biome, AnimationCurve curve, int range)
    {
        nodes.Clear();

        foreach (var (coord, node) in graph)
        {
            if (node == null)
                continue;

            NodeData data = new NodeData();
            
            //if (!HexGrid.IsValidGridPosition(coord, range))
            //    continue;

            data.position = coord;

            var neighbors = new List<Vector2Int>();

            foreach(var n in node.neighbors)
            {
                neighbors.Add(n.cell.coord);
            };

            data.neighbors = new List<Vector2Int>();
            data.neighbors.AddRange(neighbors);

            if (heightmap.ContainsKey(coord))
            {
                data.height = heightmap[coord];
            }

            // find the corrisponding type 
            int idx = 0;

            for (int i = 0; i < biome.Count; i++)
            {
                if (!heightmap.ContainsKey(coord))
                    continue;

                float currentHeight = heightmap[coord];

                if (currentHeight <= biome[i].height)
                {
                    idx = i;
                    break;
                }
            }

            data.type = biome[idx];

            enviormentCurve = curve;
            
            nodes.Add(data);
        }


    }
    


} 
