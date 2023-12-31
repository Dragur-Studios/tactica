﻿using System.Collections.Generic;
using UnityEngine;


public class AStar
{
    const int STANDARD_COST = 10;
    const int DIAGONAL_COST = 14;


    public static List<Vector2Int> FindPath(HexPathfinder pathfinder, Vector2Int start, Vector2Int target)
    {

        var openSet = new List<AStarNode>();
        var closedSet = new HashSet<AStarNode>();
       
        var startNode = pathfinder.nodes[start];       // Use the existing node from the grid
        var targetNode = pathfinder.nodes[target];    // Use the existing node from the grid

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet[0];
            
            // Prefer Lower Costing Nodes
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            Debug.Log($"Checking: {currentNode}");


            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                Debug.Log("Path Found");
                return RetracePath(startNode, targetNode);
            }

            var neighbors = HexGrid.GetNeighborPositions(currentNode.position, 16);
            
            foreach (var neighborPosition in neighbors)
            {
                var neighborNode = pathfinder.nodes[neighborPosition]; // Use the existing node from the grid

                Debug.Log($"Checking Neighbor: {neighborNode}");
                
                if (!neighborNode.isWalkable || closedSet.Contains(neighborNode))
                {
                    Debug.Log($"Checking Neighbor Failed: isWalkable: {neighborNode.isWalkable} isInSet:{closedSet.Contains(neighborNode)}");
                    continue;
                }

                var newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighborNode);

                if (newCostToNeighbor < neighborNode.gCost || !openSet.Contains(neighborNode))
                {
                    neighborNode.gCost = newCostToNeighbor;
                    neighborNode.hCost = GetDistance(neighborNode, targetNode);
                    neighborNode.parent = currentNode;

                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }

        Debug.Log("<color=red>NO PATH COULD BE DETERMINED!</color>");
        return null; // Path not found
    }

    private static List<Vector2Int> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        var path = new List<Vector2Int>();
        var currentNode = endNode;

        Debug.Log("Retracing Steps");
        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Add(startNode.position);

        path.Reverse();
        return path;
    }
    private static int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int dx = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int dy = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        int rem = Mathf.Abs(dx - dy);
        return DIAGONAL_COST * Mathf.Min(dx, dy) + STANDARD_COST * rem;
    }

}
