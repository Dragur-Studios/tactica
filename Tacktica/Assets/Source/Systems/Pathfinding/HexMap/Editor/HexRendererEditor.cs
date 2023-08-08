using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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

        var mesh = hexagon.GetComponent<MeshFilter>().sharedMesh;
        if(mesh == null)
        {
            if (GUILayout.Button("Generate"))
            {
               
                hexagon.GenerateMesh();
                mesh = hexagon.GetComponent<MeshFilter>().sharedMesh;
            }
        }
        else
        {

            if (GUILayout.Button("Save Mesh"))
            {
                hexagon.GenerateMesh();
                
                string basepath = "Assets/Resources/Meshes/";
                if (!Directory.Exists(basepath))
                {
                    AssetDatabase.CreateFolder("Assets/Resources/", "Meshes");
                }

                AssetDatabase.CreateAsset(mesh, $"{basepath}hexagon.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }
        }
    
    }
}
