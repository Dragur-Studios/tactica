using System.Collections.Generic;
using UnityEngine;

public class HexPathfinder : MonoBehaviour
{
    public Dictionary<Vector2Int, AStarNode> nodes;

    HexWorldGenerator layout;

    public List<Vector3> FindPath(Vector2Int start, Vector2Int target)
    {
        if (layout == null)
            layout = GetComponent<HexWorldGenerator>();

        nodes = new Dictionary<Vector2Int, AStarNode>();

        for (int x = -layout.range; x < layout.range; x++)
        {
            for (int y = -layout.range; y < layout.range; y++)
            {
                var coord = new Vector2Int(x, y);
               
                var node = new AStarNode(layout, new Vector2Int(x, y), layout.graph[coord].cell.isWalkable);

                nodes.Add(coord, node);
            }
        }
        var gridPath = AStar.FindPath(this, start, target);
        return path = ConvertPathToWorld(gridPath);
    }

    private List<Vector3> ConvertPathToWorld(List<Vector2Int> gridPath)
    {
        List<Vector3> converted = new List<Vector3>();


        for (int i = 0; i < gridPath.Count; i++)
        {
            var coord = new Vector2Int(gridPath[i].x, gridPath[i].y);
            converted.Add(layout.graph[coord].cell.center.position);
        }
        
        return converted;
    }


    private bool IsValidGridPosition(Vector2Int position)
    {
        if (layout == null)
            layout = GetComponent<HexWorldGenerator>();


        // Ensure that both x and y coordinates are non-negative
        if (position.x < -layout.range || position.y < -layout.range)
        {
            return false;
        }

        // Check if the position is within the grid resolution
        if (position.x >= layout.range || position.y >= layout.range)
        {
            return false;
        }

        if (nodes == null)
            return false;

        
        if(!nodes[position].isWalkable)
        {
            return false;
        }

        return true;
    }


    private void OnDrawGizmos()
    {
        if (path == null)
            return;

        bool hasPath = path.Count > 0;

        if (hasPath)
        {
            var _start = path[0];
            var _end = path[path.Count-1];

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_start, _start + Vector3.up);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_end, _end + Vector3.up);

            for (int i = 0; i < path.Count - 1; i++)
            {
                var p1 = path[i];
                var p2 = path[i + 1];

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p1, p2);

            }

        }

    }

    List<Vector3> path = new List<Vector3>();

}
