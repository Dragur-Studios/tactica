using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(HexRenderer))]
public class HexRendererEditor : Editor
{
    HexRenderer hexagon;
    
    private void OnEnable()
    {
        hexagon = (HexRenderer)target;
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    
    }
}
