using UnityEngine;

public class HexagonCell : MonoBehaviour 
{

    public bool hover = false;

    MeshRenderer renderer;
    Material material;

    Color defaultColor = Color.white;

    HexagonGrid grid;
    internal bool selected;

   
    public void Init(HexagonGrid grid)
    {
        renderer = GetComponent<MeshRenderer>();
        material = renderer.material;

        defaultColor = renderer.material.GetColor("_Base_Color");
        this.grid = grid;
    }

    private void Update()
    {
        if (hover)
        {
            material.SetColor("_Base_Color", Color.green);
        }
        else if (selected)
        {
            material.SetColor("_Base_Color", Color.blue);
        }
        else
        {
            material.SetColor("_Base_Color", defaultColor);
        }
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
