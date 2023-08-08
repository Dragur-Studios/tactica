using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public enum HexState
{
    None,
    Hover,
    Selected,

    Occupied,
    Occupied_Hover,
    Occupied_Selected,
    
    __COUNT__
}

public class HexagonCell : MonoBehaviour 
{
    public bool hover = false;
    public Vector2Int coord = Vector2Int.zero;

    // Color defaultColor = Color.white;
    [HideInInspector] public Transform center = null;

    public Agent agent = null;

    public bool isSelected = false;
    public bool isWalkable = true;
    public bool isOccupied = false;

    HexNodeDisplay nodeDisplay = null;
    HexWorld world;

    public void Init(HexWorld world, Vector2Int coord)
    {
        if(world != null)
        {
            this.world = world;    
        }

        this.coord = coord;

        nodeDisplay = GetComponentInChildren<HexNodeDisplay>();
    }

    private void Update()
    {
        if (nodeDisplay == null)
        {
            nodeDisplay = GetComponentInChildren<HexNodeDisplay>();
            return;
        }

        var col = Physics.OverlapSphere(center.transform.position, 0.8f);
        isOccupied = col.Length > 1;

        if (!isOccupied)
        {
            if (hover)
            {
                nodeDisplay.SetState(HexState.Hover);
            }
            else if (isSelected)
            {
                nodeDisplay.SetState(HexState.Selected);
            }
            else
            {
                nodeDisplay.SetState(HexState.None);
            }

        }
        else
        {
            for (int i = 0; i < col.Length; i++)
            {
                col[i].TryGetComponent<Agent>(out agent);
            }


            if (hover)
            {
                nodeDisplay.SetState(HexState.Occupied_Hover);
            }
            else if (isSelected)
            {
                nodeDisplay.SetState(HexState.Occupied_Selected);
            }
            else
            {
                nodeDisplay.SetState(HexState.Occupied);
            }
            

        }

        if (hover && Mouse.current.leftButton.wasPressedThisFrame)
        { 
            world.OnCellSelected(this);
        }
    
    }

    private void OnMouseEnter()
    {
        world.OnCellHoverEnter(this);

        
    }


    private void OnMouseExit()
    {
        world.OnCellHoverExit(this);
    }

    
}
