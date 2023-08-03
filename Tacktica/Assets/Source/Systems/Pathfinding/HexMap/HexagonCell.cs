using UnityEngine;

public class HexagonCell : MonoBehaviour 
{

    public bool hover = false;

    MeshRenderer renderer;
    Color defaultColor = Color.white;


    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();

        defaultColor = renderer.material.GetColor("_Base_Color");
    }

    private void Update()
    {
        if (hover)
        {
            renderer.material.SetColor("_Base_Color", Color.green);
        }else
        {
            renderer.material.SetColor("_Base_Color", defaultColor);
        }
    }

    private void LateUpdate()
    {
        hover = false;
    }
}
