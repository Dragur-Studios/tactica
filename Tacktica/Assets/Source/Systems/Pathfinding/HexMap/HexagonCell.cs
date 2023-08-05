using UnityEngine;
using System.Collections.Generic;

public class HexagonCell : MonoBehaviour 
{
    public bool hover = false;
    public Vector2Int coord = Vector2Int.zero;

    // Color defaultColor = Color.white;
    [HideInInspector] public Transform center = null;
    
    //Material material;

    HexWorldGenerator grid;

    internal bool selected;
    internal bool isWalkable = true;

    //private void Start()
    //{
    //    Init();
    //}

    public void Init(Vector2Int coord)
    {
          
        if (grid == null)
            grid = FindObjectOfType<HexWorldGenerator>();


        this.coord = coord;

    }

    private void Update()
    {
        //if (renderer == null)
        //    return;

        //if (hover)
        //{
        //    Debug.Log($"Hovering: hexagon->{name}");
        //    material.SetColor("_Base_Color", Color.green);
        //}
        //else if (selected)
        //{
        //    material.SetColor("_Base_Color", Color.blue);
        //}
        //else
        //{
        //    material.SetColor("_Base_Color", defaultColor);
        //}
    }


    public void OnMouseEnter()
    {
        grid.OnCellHoverEnter(this);
    }

    public void OnMouseExit()
    {
        grid.OnCellHoverExit(this);
    }


}
