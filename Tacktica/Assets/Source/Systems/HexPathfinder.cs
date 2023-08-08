using System.Collections.Generic;
using UnityEngine;

public static class HexPathfinder
{
    public static List<HexagonCell> FindPath(Dictionary<Vector2Int, HexNode> graph, Vector2Int start, Vector2Int target)
    {
        var nodes = new Dictionary<Vector2Int, AStarNode>();

        foreach (var (coord, node) in graph)
        {
            var astarNode = new AStarNode(node, coord);
            nodes.Add(coord, astarNode);
        }

        var path = AStar.FindPath(nodes, start, target);
        
        return GetPathCells(graph, path);
    }

    private static List<HexagonCell> GetPathCells(Dictionary<Vector2Int, HexNode> graph, List<Vector2Int> coords)
    {
        List<HexagonCell> pathCells = new List<HexagonCell>();


        for (int i = 0; i < coords.Count; i++)
        {
            var coord = new Vector2Int(coords[i].x, coords[i].y);
            pathCells.Add(graph[coord].cell);
        }
        
        return pathCells;
    }

}
