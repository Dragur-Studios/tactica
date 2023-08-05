using UnityEngine;

public class Node
{
    public Node(HexWorldGenerator layout, Vector2Int position)
    {
        this.layout = layout;
        this.position = position;
    }
    public Vector2Int position;
    
    
    protected HexWorldGenerator layout;
}


public class AStarNode : Node
{
    public AStarNode(HexWorldGenerator layout, Vector2Int position, bool isWalkable)
        : base(layout, position)
    {
        this.isWalkable = isWalkable;
    }

    public AStarNode parent;
    
    public bool isWalkable = true;

    public float gCost;
    public float hCost;
    public float FCost => gCost + hCost;
    
    
}
