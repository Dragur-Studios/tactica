using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HexWorld : MonoBehaviour
{
    public HashSet<KeyValuePair<Vector2Int, HexNode>> graph = new HashSet<KeyValuePair<Vector2Int, HexNode>>();
    public HexWorldData worldData;
    
    HexagonCell selected;
    
    private void Awake()
    {
        if (worldData == null)
        {
            Debug.Log("<color=red>Error! World Data is Null!</color>");
            return;
        }

        var start = Time.realtimeSinceStartup;
        

        StartCoroutine(BuildGraph());

        var end = Time.realtimeSinceStartup;

        Debug.Log($"Took {end - start}s to generate!");
    }

    IEnumerator BuildGraph()
    {
        var coords = new List<Vector2Int>();

        worldData.nodes.ForEach(node => coords.Add(node.position));

        for (int i = 0; i < coords.Count; i++)
        {
            var node = new HexNode();
            var data = worldData.nodes[i];
            
            node.cell = HexGrid.CreateCell(transform, HexWorldData.MaxHeight * worldData.enviormentCurve.Evaluate(data.height), coords[i], data.type, worldData.isFlatTopped);
            node.cell.gameObject.isStatic = true;
            
            AddCenter(node);
            AddUI(node);
            AddCollider(node);

            graph.Add(new KeyValuePair<Vector2Int, HexNode>(coords[i], node));
        }

        yield return null;

        for (int i = 0; i < coords.Count; i++)
        {
            var node = graph.First(n => n.Key == coords[i]).Value;

            var neighbors = worldData.nodes[i].neighbors;

            for (int j = 0; j < neighbors.Count; j++)
            {
                var n = graph.First(n => n.Key == neighbors[j]).Value;

                if (n != null)
                {
                    node.neighbors.Add(n);
                }
            }

        }

        yield return null;

    }

    internal Vector3 RandomPosition()
    {
        List<Vector2Int> coords = new List<Vector2Int>();

        foreach (var (coord, n) in graph)
        {
            coords.Add(coord);
        }

        var idx = UnityEngine.Random.Range(0, coords.Count - 1);

        var node = graph.First(n => n.Key == coords[idx]).Value;

        return node.cell.center.position;

    }

    private void AddCollider(HexNode node)
    {
        var cell = node.cell;

        if (cell.TryGetComponent(out MeshCollider col))
        {
            col.convex = true;
            col.sharedMesh = cell.GetComponent<HexRenderer>().GetMesh();
        }
        else
        {
            col = cell.gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            col.sharedMesh = cell.GetComponent<HexRenderer>().GetMesh();
        }
    }

    private void AddUI(HexNode node)
    {
        var go = new GameObject("ui");
        go.transform.position = node.cell.center.position + (Vector3.one * 1e-6f);
        go.transform.SetParent(node.cell.transform, true);


        // add the pathfinding UI object
        var node_display = new GameObject("node-display", typeof(HexNodeDisplay), typeof(HexRenderer));
        var hex = node_display.GetComponent<HexRenderer>();
        hex.isFlatTopped = worldData.isFlatTopped;
        hex.outerSize = 1.0f;
        hex.innerSize = 0.9f;
        hex.height = 0.1f;
        hex.GenerateMesh();
        node_display.transform.position = go.transform.position;
        node_display.transform.SetParent(go.transform, true);

        hex.Material = Resources.Load<Material>("Materials/Tiles/Indicator");
        hex.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        //node_display.SetActive(false);

        // add the node position ui object
        // add any other UI objects here



    }

    private static void AddCenter(HexNode node)
    {
        node.cell.center = new GameObject("center").transform;

        // set the position to the cells transform to start alignment
        node.cell.center.position = node.cell.transform.position;

        // align the center object to the top of the cell
        var center = node.cell.center.position;
        
        if (node.cell.TryGetComponent(out HexRenderer hex))
            center.y += (hex.height) + 1e-4f;   // use the height and some small offset to get around z-fighting
        
        node.cell.center.position = center;
        node.cell.center.SetParent(node.cell.transform, true);

    }



    public void OnCellHoverEnter(HexagonCell cell)
    {
        cell.hover = true;
        //hover = cell;
    }

    public void OnCellHoverExit(HexagonCell cell)
    {
        //hover = null;
        cell.hover = false;
    }

    public void OnCellSelected(HexagonCell cell)
    {
        if (selected == cell)
        {
            cell.isSelected = false;
            selected = null;
            return;
        }


        if (selected != null)
        {
            selected.isSelected = false;
            selected = null;
        }

        cell.isSelected = true;
        selected = cell;

    }


}
