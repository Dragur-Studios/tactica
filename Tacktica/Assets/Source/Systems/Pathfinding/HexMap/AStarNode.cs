using UnityEngine;

public class Node
{
    public Node(HexagonGrid layout, Vector2Int position)
    {
        this.layout = layout;
        this.position = position;
    }
    public Vector2Int position;
    
    
    protected HexagonGrid layout;
}


public class AStarNode : Node
{
    public AStarNode(HexagonGrid layout, Vector2Int position)
        : base(layout, position)
    {

    }

    public AStarNode parent;

    public bool IsWalkable() {
        return Physics.OverlapSphere(layout.GridToWorldPosition(position), 0.8f)?.Length <= 0;
    }

    public float gCost;
    public float hCost;
    public float FCost => gCost + hCost;
    
    
}
