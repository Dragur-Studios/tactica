using System.Collections.Generic;
using UnityEngine;

public class HexPathfinder : MonoBehaviour
{
    public AStarNode[,] nodes;

    HexagonGrid layout;

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        if (layout == null)
            layout = GetComponent<HexagonGrid>();

        // copy all of the tiles into nodes.
        var height = layout.grid.GetLength(1);
        var width = layout.grid.GetLength(0);

        nodes = new AStarNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = new AStarNode(layout, new Vector2Int(x, y));

                nodes[x, y] = node;
            }
        }

        return path = AStar.FindPath(this, start, target);
    }
    public List<Vector2Int> GetNeighborPositions(Vector2Int position)
    {
        if (layout == null)
            layout = GetComponent<HexagonGrid>();

        List<Vector2Int> neighbors = new List<Vector2Int>();


        bool oddRow = position.y % 2 == 1;

        if(position.x -1 >= 0)
        {// left 
            neighbors.Add(new Vector2Int(position.x - 1, position.y));

        }
        if (position.x + 1 < layout.gridResolution)
        {// right 
            neighbors.Add(new Vector2Int(position.x + 1, position.y));

        }
        if(position.y -1 >= 0)
        {// down
            neighbors.Add(new Vector2Int(position.x, position.y-1));

        }
        if(position.y + 1 < layout.gridResolution)
        {// up
            neighbors.Add(new Vector2Int(position.x, position.y+1));
        }

        if (oddRow)
        {
            if(position.y + 1 < layout.gridResolution && position.x + 1 < layout.gridResolution)
            {
                neighbors.Add(new Vector2Int(position.x + 1, position.y + 1));
            }
            if(position.y -1 >= 0 && position.x + 1 < layout.gridResolution)
            {

                neighbors.Add(new Vector2Int(position.x + 1, position.y - 1));
            }
        }
        else
        {
            if(position.y + 1 < layout.gridResolution && position.x -1 >= 0)
            {
                neighbors.Add(new Vector2Int(position.x-1, position.y+1));
            }
            if(position.y -1 >= 0 && position.x -1 >= 0)
            {
                neighbors.Add(new Vector2Int(position.x-1, position.y-1));
            }
        }


        return neighbors;
    }


    private bool IsValidGridPosition(Vector2Int position)
    {
        if (layout == null)
            layout = GetComponent<HexagonGrid>();


        // Ensure that both x and y coordinates are non-negative
        if (position.x < 0 || position.y < 0)
        {
            return false;
        }

        // Check if the position is within the grid resolution
        if (position.x >= layout.gridResolution || position.y >= layout.gridResolution)
        {
            return false;
        }

        if (nodes == null)
            return false;

        if(!nodes[position.x, position.y].IsWalkable())
        {
            return false;
        }

        return true;
    }


    private void OnDrawGizmos()
    {
        bool hasPath = path.Count > 0;

        if (hasPath)
        {
            var _start = layout.GridToWorldPosition(path[0]);
            var _end = layout.GridToWorldPosition(path[path.Count-1]);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(_start, _start + Vector3.up);

            var positions = GetNeighborPositions(path[0]);
            for (int i = 0; i < positions.Count; i++)
            {
                var p = layout.GridToWorldPosition(positions[i]);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(p, p + Vector3.up);

            }


            Gizmos.color = Color.red;
            Gizmos.DrawLine(_end, _end + Vector3.up);

            for (int i = 0; i < path.Count - 1; i++)
            {
                var p1 = layout.GridToWorldPosition(path[i]);
                var p2 = layout.GridToWorldPosition(path[i + 1]);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p1, p2);

            }




        }

    }

    List<Vector2Int> path = new List<Vector2Int>();

}
