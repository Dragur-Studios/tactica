using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HexNode
{
    public HexagonCell cell;
    public List<HexNode> neighbors = new List<HexNode>();
}

[System.Serializable]
public struct ElevationSettings
{
    public string name;
    public float height;
    public Color color;

    public static ElevationSettings Default => new()
    {
        name = "_DEFAULT_",
        height = 1e-6f,
        color = Color.black,
    };
        
    public bool isTransparent => color.a < 1.0f;

}

public static class HexGrid
{
    public const float cellSize = 1.0f;

    public static HexagonCell CreateCell(Transform parent, float height, Vector2Int coord, ElevationSettings type, bool isFlatTopped)
    {
        var go = new GameObject($"Hex( {coord})", typeof(HexagonCell), typeof(HexRenderer));
        var cell = go.GetComponent<HexagonCell>();
        cell.Init(coord);
        var hex = go.GetComponent<HexRenderer>();
        hex.isFlatTopped = isFlatTopped;
        hex.height = height;

        var material = new Material(Resources.Load<Material>("Materials/Tiles/Hexagon" + (type.isTransparent ? "(Transparent)" : "")));
        material.SetColor("_Base_Color", type.color);
        hex.Material = material;


        hex.GenerateMesh();
        
        cell.transform.position = GridToWorldPosition(coord, cellSize, hex.isFlatTopped);
        cell.transform.SetParent(parent, true);
        return cell;
    }

    public static List<Vector2Int> GetNeighborPositions(Vector2Int position, int range)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        int half_cells = Mathf.RoundToInt(range / 2);

        bool oddRow = position.y % 2 == 1;

        if (position.x - 1 >= -half_cells)
        {// left 
            neighbors.Add(new Vector2Int(position.x - 1, position.y));

        }
        if (position.x + 1 < half_cells)
        {// right 
            neighbors.Add(new Vector2Int(position.x + 1, position.y));

        }
        if (position.y - 1 >= -half_cells)
        {// down
            neighbors.Add(new Vector2Int(position.x, position.y - 1));

        }
        if (position.y + 1 < half_cells)
        {// up
            neighbors.Add(new Vector2Int(position.x, position.y + 1));
        }

        if (oddRow)
        {
            if (position.y + 1 < half_cells && position.x + 1 < half_cells)
            {
                neighbors.Add(new Vector2Int(position.x + 1, position.y + 1));
            }
            if (position.y - 1 >= -half_cells && position.x + 1 < half_cells)
            {

                neighbors.Add(new Vector2Int(position.x + 1, position.y - 1));
            }
        }
        else
        {
            if (position.y + 1 < half_cells && position.x - 1 >= -half_cells)
            {
                neighbors.Add(new Vector2Int(position.x - 1, position.y + 1));
            }
            if (position.y - 1 >= -half_cells && position.x - 1 >= -half_cells)
            {
                neighbors.Add(new Vector2Int(position.x - 1, position.y - 1));
            }
        }

        return neighbors;
    }
    
    public static Vector3 GridToWorldPosition(Vector2Int coord, float cellsize, bool isFlatTopped)
    {
        float row = coord.x;
        float col = coord.y;
        float width;
        float height;
        float xPosition = 0;
        float yPosition = 0;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = cellsize;


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

            horizontalDistance = width * (3.0f / 4.0f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2.0f : 0;

            xPosition = (col * horizontalDistance);
            yPosition = (row * verticalDistance) - offset;

        }



        return new Vector3(xPosition, 0, yPosition);
    }

    public static bool IsValidGridPositionRect(Vector2Int position, Rect rect)
    {
        return position.x >= rect.x && position.x < rect.x + rect.width &&
               position.y >= rect.y && position.y < rect.y + rect.height;
    }

    public static bool IsValidGridPosition(Vector2Int position, int radius)
    {
        int centerX = 0; // Set the center of the circle as needed
        int centerY = 0;

        float distance = Vector2Int.Distance(position, new Vector2Int(centerX, centerY));

        return distance <= radius;
    }

}


public class HexWorldGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int cell_count = 999;
    

    [Header("Visibility")]
    public int range = 16;

    [Header("Tile Settings")]
    public bool isFlatTopped = false;
    public float inner_size = 0.75f;
    public float outer_size = 1.0f;
    public float height = 0.1f;

    public Dictionary<Vector2Int, HexNode> graph;
    public Dictionary<Vector2Int, float> heightmap;

    HexagonCell hover;
    HexagonCell selected;

    // enviorment settings
    public List<ElevationSettings> biome = new List<ElevationSettings>();
   
    [HideInInspector] public int seed = 0;
    [HideInInspector] public GeoGenAlgorithm algorithm = GeoGenAlgorithm.Perlin;


    // diamond square settings
    [HideInInspector] public int mapSize = 64;          // Make sure it's 2^n + 1 for the Diamond-Square algorithm
    [HideInInspector] public float roughness = 0.5f;

    // perlin settings
    [HideInInspector] public const float minHeightMultiplier = 1.0f;
    [HideInInspector] public const float maxHeightMultiplier = 100.0f;
    [HideInInspector] public float maxHeight = 10f;
    [HideInInspector] public AnimationCurve heightMultiplierCurve = new AnimationCurve();
    [HideInInspector] public const float minLacunarity = 1e-9f;
    [HideInInspector] public const float maxLacunarity = 10.0f;
    [HideInInspector] public float lacunarity = 0.1f;
    [HideInInspector] public const float minPersistance = 1e-9f;
    [HideInInspector] public const float maxPersitance = 1.0f;
    [HideInInspector] public float persistance = 1.0f;
    [HideInInspector] public const float minScale = 1e-9f;
    [HideInInspector] public const float maxScale = 10.0f;
    [HideInInspector] public float scale = 0.17f;
    [HideInInspector] public const int minOctaves = 1;
    [HideInInspector] public const int maxOctaves = 10;
    [HideInInspector] public int octaves = 4;

    // Serialization Information
    [HideInInspector] public string worldName;

    public void Bake(string worldName)
    {
        HexWorld world = new GameObject($"{worldName}", typeof(HexWorld)).GetComponent<HexWorld>();
        
        var worldData = ScriptableObject.CreateInstance<HexWorldData>();
        worldData.name = worldName;
        worldData.Decompose(graph, heightmap, biome, heightMultiplierCurve, range*2);

        world.worldData = worldData;
        
        if (!Directory.Exists("Assets/Hex Worlds/"))
        {
            AssetDatabase.CreateFolder("Assets", "Hex Worlds");
        }

        string basepath = "Assets/Hex Worlds/";
        string prefabPath = Path.Combine(basepath, $"{world.gameObject.name}.prefab");
        string dataPath = Path.Combine(basepath, $"{worldData.name}.asset");
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

        var pref = PrefabUtility.SaveAsPrefabAsset(world.gameObject, prefabPath, out bool succes);

        if (succes)
        {
            Debug.Log("Bake <color=green>SUCCESS!</color>");
        }
        else
        {

            Debug.Log("Bake <color=red>FAILED!</color>");
        }
        pref.GetComponent<HexWorld>().worldData = worldData;
        // Save the WorldData file 
        AssetDatabase.CreateAsset(worldData, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        DestroyImmediate(world.gameObject);

    }
    public void GenerateGrid(bool meshes, bool waypoints, bool colliders, bool ui_anchor)
    {
        Clear();

        graph = new Dictionary<Vector2Int, HexNode>();

        CreateCells();
        ConnectNeighbors();

        if (meshes)
        {
            GenerateMeshes();
        }

        if (waypoints)
        {
            GenerateWaypoints();
        }

        if (colliders)
        {
            GenerateCollision();
        }

        if (ui_anchor)
        {
            GenerateGUIAnchor();
        }

        Debug.Log("<color=green> Generated Grid</color>");
    }

    private void CreateCells()
    {
        int half_cells = Mathf.RoundToInt(cell_count / 2);

        for (int x = -half_cells; x < half_cells; x++)
        {
            for (int y = -half_cells; y < half_cells; y++)
            {

                var coord = new Vector2Int(x, y);
                var node = new HexNode();
                
                if (!HexGrid.IsValidGridPosition(coord, range))
                    continue;

                if(graph.TryGetValue(coord, out HexNode found))
                {          
                    continue;
                }
                else
                {
                    node.cell = HexGrid.CreateCell(transform, 0.1f, coord, ElevationSettings.Default, isFlatTopped);
                    
                    graph.Add(new Vector2Int(x, y), node);
                }
            }
        }
    }

    private void ConnectNeighbors()
    {
        foreach (var (coord, node) in graph)
        {
            var neighbors = HexGrid.GetNeighborPositions(coord, graph.Count);

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (HexGrid.IsValidGridPosition(neighbors[i], range))
                {
                    node.neighbors.Add(graph[neighbors[i]]);
                }
            }

        }
    }

    public void GenerateGrid(Dictionary<Vector2Int, float> heights, bool meshes, bool waypoints, bool colliders, bool ui_anchor)
    {
        Clear();

        graph = new Dictionary<Vector2Int, HexNode>();
        heightmap = heights;

        CreateCells();
        ConnectNeighbors();

        if (meshes)
        {
            GenerateMeshes(heightmap);
        }

        if (waypoints)
        {
            GenerateWaypoints();
        }

        if (colliders)
        {
            GenerateCollision();
        }

        if (ui_anchor)
        {
            GenerateGUIAnchor();
        }
    }

    void GenerateMeshes()
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                var coord = new Vector2Int(x, y);
                var node = graph[coord];
                var cell = node.cell;

                if (!HexGrid.IsValidGridPosition(coord, range))
                    continue;

                if (cell.TryGetComponent(out HexRenderer hex))
                {
                    hex.innerSize = inner_size;
                    hex.outerSize = outer_size;
                    hex.height = height;
                    hex.isFlatTopped = isFlatTopped;
                    hex.GenerateMesh();
                }
                else
                {
                    hex = cell.gameObject.AddComponent<HexRenderer>();
                    hex.innerSize = inner_size;
                    hex.outerSize = outer_size;
                    hex.height = height;
                    hex.isFlatTopped = isFlatTopped;
                    hex.GenerateMesh();

                }
            }
        }
    }

    void GenerateMeshes(Dictionary<Vector2Int, float> heightmap)
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {   

                var coord = new Vector2Int(x, y);
                
                if (!HexGrid.IsValidGridPosition(coord, range))
                    continue;

                var node = graph[coord];

                var height = heightmap[coord];

                if (height <= 0)
                    height = 1e-6f;

                if (node.cell.TryGetComponent(out HexRenderer hex))
                {
                    hex.innerSize = inner_size;
                    hex.outerSize = outer_size;
                    hex.height = maxHeight * heightMultiplierCurve.Evaluate(height);
                    hex.isFlatTopped = isFlatTopped;
                    hex.GenerateMesh();

                   
                }
                else
                {
                    hex = node.cell.gameObject.AddComponent<HexRenderer>();
                    hex.innerSize = inner_size;
                    hex.outerSize = outer_size;
                    hex.height = maxHeight * heightMultiplierCurve.Evaluate(height);
                    hex.isFlatTopped = isFlatTopped;
                    hex.GenerateMesh();
                }

                var material = new Material(Resources.Load<Material>("Materials/Tiles/Hexagon" + (heightmap[coord] <= 0.4f ? "(Transparent)" : "")));


                // find the color that corrisponds with the height value
                // heights are normalized 
                Color color = Color.magenta; // WRONG COLOR

                for (int i = 0; i < biome.Count; i++)
                {
                    float currentHeight = heightmap[coord];

                    if (currentHeight <= biome[i].height )
                    {
                        color = biome[i].color;
                        //Debug.Log($"Color Found: height(norm): {heightmap[coord]} height: {height} color: {color}  ");
                        break;
                    }
                }

                material.SetColor("_Base_Color", color);

                hex.Material = material;

            }
        }
    }

    void GenerateWaypoints()
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                var coord = new Vector2Int(x, y);

                if (!HexGrid.IsValidGridPosition(coord, range))
                    continue;

                var node = graph[coord];
                var cell = node.cell;

                // create the center transform object for use with pathfinding and placement
                cell.center = new GameObject("center").transform;

                // set the position to the cells transform to start alignment
                cell.center.position = cell.transform.position;

                // align the center object to the top of the cell
                var center = cell.center.position;
                if (cell.TryGetComponent(out HexRenderer hex))
                    center.y += (hex.height) + 1e-4f;   // use the height and some small offset to get around z-fighting
                cell.center.position = center;
                cell.center.SetParent(cell.transform, true);
            }
        }
    }

    void GenerateGUIAnchor()
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                var coord = new Vector2Int(x, y);
                var node = graph[coord];
                var cell = node.cell;


                if (!HexGrid.IsValidGridPosition(coord, range))
                    continue;

                var hex = cell.GetComponent<HexRenderer>();

                // Create the Debug Text. and Assign it to the Cell
                var ui_anchor = new GameObject($"ui_anchor{x}{y}", typeof(TMPro.TextMeshPro));
                {
                    var txt = ui_anchor.GetComponent<TMPro.TextMeshPro>();
                    txt.text = $"{x},{y}";
                    txt.fontSize = 4;
                    txt.alignment = TMPro.TextAlignmentOptions.Center;
                }
                // align the debug text object to the cells position to start alignment
                ui_anchor.transform.position = cell.transform.position;

                // align the debug text object to the top of the cell
                var pos = ui_anchor.transform.position;
                pos.y += (hex.height) + 1e-4f;  // use the height and some small offset to get around z-fighting
                ui_anchor.transform.position = pos;

                var rect = ui_anchor.GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);

                ui_anchor.transform.Rotate(new Vector3(90f, 0, 0));
                ui_anchor.transform.SetParent(cell.transform, true);
            }
        }

    }

    void GenerateCollision()
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                var coord = new Vector2Int(x, y);
                var node = graph[coord];
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
        }
    }

    void RemoveCollision()
    {
        for (int x = -range; x < range; x++)
        {
            for (int y = -range; y < range; y++)
            {
                var coord = new Vector2Int(x, y);
                var node = graph[coord];
                var cell = node.cell;

                if (cell.TryGetComponent(out MeshCollider col))
                {
                    DestroyImmediate(col);
                }
            }
        }
    }




  
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        float size = HexGrid.cellSize;

        float width = Mathf.Sqrt(3) * size;
        float height = 2f * size;
        float col, row;

        if (!isFlatTopped)
        {
            col = (worldPosition.x / width);
            row = ((worldPosition.z + ((Mathf.Floor(col) % 2 == 0) ? 0 : height * 0.75f)) / height) - 0.5f;
        }
        else
        {
            row = (worldPosition.x / width);
            col = ((worldPosition.z + ((Mathf.Floor(row) % 2 == 0) ? 0 : height * 0.5f)) / height) - 0.5f;
        }

        int roundedCol = Mathf.RoundToInt(col);
        int roundedRow = Mathf.RoundToInt(row);

        if (!isFlatTopped)
        {
            int temp = roundedCol;
            roundedCol = roundedRow;
            roundedRow = temp;
        }

        Vector2Int gridPosition = new Vector2Int(roundedCol, roundedRow);
        return gridPosition;
    }

    public void Clear()
    {

        var child_count = transform.childCount;
        for (int i = child_count - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }


        if (graph == null)
            return;

        graph.Clear();

    }






    public void OnCellHoverEnter(HexagonCell cell)
    {
        cell.hover = true;
        hover = cell;
    }

    public void OnCellHoverExit(HexagonCell cell)
    {
        hover = null;
        cell.hover = false;
    }

    public void OnCellSelected(HexagonCell cell)
    {
        if (selected == cell)
        {
            cell.selected = false;
            selected = null;
            return;
        }

        if (selected)
        {
            selected.selected = false;
            selected = null;
        }

        cell.selected = true;
        selected = cell;

        if (hover)
        {
            hover = null;
        }
    }


}
