using UnityEngine;

public class Node
{
    public Node(HexNode data, Vector2Int position)
    {
        this.data = data;
        this.position = position;
    }
    public Vector2Int position;
    
    protected HexNode data;
}


public class AStarNode : Node
{
    public AStarNode(HexNode data, Vector2Int position)
        : base(data, position)
    {
        isWalkable = data.cell.isWalkable;
    }

    public AStarNode parent;
    
    public bool isWalkable = true;

    public float gCost;
    public float hCost;
    public float FCost => gCost + hCost;
    
    
}
