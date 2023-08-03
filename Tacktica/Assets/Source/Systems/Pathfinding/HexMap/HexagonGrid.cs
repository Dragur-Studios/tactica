using System;
using System.Collections.Generic;
using UnityEngine;


public class HexagonGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridResolution = 6;
    public float cellSize = 0;

    [Header("Tile Settings")]
    public bool isFlatTopped = false;
    public float inner_size = 0.75f;
    public float outer_size = 1.0f;
    public float height = 0.1f;

    public HexagonCell[,] grid;         // rendering

    public bool DebugMode = true;

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 10000.0f))
        {
            Vector2Int gridPosition = WorldToGridPosition(hit.point);
              

            // Check if the grid position is within the valid range of your grid
            if (IsValidGridPosition(gridPosition))
            {
                var hex = grid[gridPosition.x, gridPosition.y];
                hex.hover = true;

            }
                
        }
        
    }

    private bool IsValidGridPosition(Vector2Int position)
    { 
        return position.x >= 0 && position.x < gridResolution &&
               position.y >= 0 && position.y < gridResolution;
    }


    public void GenerateGrid()
    {
        ClearGrid();

     
        //nodes = new AStarNode[gridResolution, gridResolution];
        grid = new HexagonCell[gridResolution, gridResolution];

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                var go = new GameObject($"Hex{x}{y}", typeof(HexagonCell), typeof(HexRenderer));

                var hex = go.GetComponent<HexRenderer>();
                hex.innerSize = inner_size;
                hex.outerSize = outer_size;
                hex.height = height;
                hex.isFlatTopped = isFlatTopped;
                hex.GenerateMesh();

                go.transform.position = GetPositionByOffsets(new (x, y));
                go.transform.SetParent(transform, true);

                //nodes[x, y] = new AStarNode(new ( x, y), true);
                var cell = go.GetComponent<HexagonCell>();

                if (DebugMode)
                {
                    // Create the Debug Text. and Assign it to the Cell
                    var dbgText = new GameObject($"dbg_text_hex_{x}{y}", typeof(TMPro.TextMeshPro));
                    var txt = dbgText.GetComponent<TMPro.TextMeshPro>();
                    txt.text = $"{x},{y}";
                    txt.fontSize = 4;
                    txt.alignment = TMPro.TextAlignmentOptions.Center;
                    
                    dbgText.transform.position = cell.transform.position;
                    
                    var pos = txt.transform.position;
                    pos.y += (hex.height / 2) + 1e-4f;
                    txt.transform.position = pos;

                    var rect = txt.GetComponent<RectTransform>();
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
                
                    dbgText.transform.Rotate(new Vector3(90f, 0, 0));
                    dbgText.transform.SetParent(cell.transform, true);
                }


                grid[x, y] = cell;

            }
        }

        Debug.Log("<color=green> Generated Grid</color>");
    }


    private Vector3 GetPositionByOffsets(Vector2Int coord)
    {
        float row = coord.x;
        float col= coord.y;
        float width;
        float height;
        float xPosition = 0;
        float yPosition = 0;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = cellSize;


        if (!isFlatTopped)
        {
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size; 

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = shouldOffset ? width / 2.0f : 0;

            xPosition = (col * horizontalDistance) + offset;
            yPosition = (row * verticalDistance);
        }
        else
        {
            shouldOffset = (col % 2) == 0;

            height = Mathf.Sqrt(3) * size;
            width = 2f * size;

            horizontalDistance = width * (3.0f/4.0f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2.0f : 0;

            xPosition = (col * horizontalDistance) ;
            yPosition = (row * verticalDistance) - offset;

        }

  

        return new Vector3(xPosition, 0, -yPosition);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return GetPositionByOffsets(gridPosition);
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        float size = cellSize;
        float width;
        float height;

        float col, row;

        if (!isFlatTopped)
        {
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;
            col = (worldPosition.x / (width * 0.75f)) + 0.5f;
            row = (worldPosition.z / height) + 0.5f;
        }
        else
        {
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;
            col = (worldPosition.x / width) + 0.5f;
            row = ((worldPosition.z + ((Mathf.Floor(col) % 2 == 0) ? 0 : height * 0.5f)) / height) + 0.5f;
        }

        int roundedCol = Mathf.RoundToInt(col);
        int roundedRow = Mathf.RoundToInt(row);

        Vector2Int gridPosition = new Vector2Int(roundedCol, roundedRow);

        Debug.Log($"World Position: {worldPosition}, Grid Position: {gridPosition}");

        return gridPosition;
    }





    private void ClearGrid()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        if (grid == null)
            return;

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                if (grid[x, y] == null)
                    continue;

                DestroyImmediate(grid[x, y]);
                grid[x, y] = null;
            }
        }

       
    }
}
