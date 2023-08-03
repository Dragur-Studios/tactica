using UnityEngine;

public class HexagonCell : MonoBehaviour 
{
    public bool hover = false;

    [HideInInspector] public Transform center = null;
    MeshRenderer renderer;
    Material material;

    Color defaultColor = Color.white;

    HexagonGrid grid;
    internal bool selected;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (!Application.isPlaying)
        {
            renderer = GetComponent<MeshRenderer>();
            material = renderer.sharedMaterial;

            defaultColor = renderer.sharedMaterial.GetColor("_Base_Color");
        }
        else
        {
            renderer = GetComponent<MeshRenderer>();
            material = renderer.material;

            defaultColor = renderer.material.GetColor("_Base_Color");
        }

        if (grid == null)
            grid = FindObjectOfType<HexagonGrid>();

    }

    private void Update()
    {
        if (hover)
        {
            Debug.Log($"Hovering: hexagon->{name}");
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
