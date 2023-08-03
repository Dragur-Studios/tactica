using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum GeoGenAlgorithm
{
    None = -1,
    Perlin = 0,
    DiamondSquare = 1
}


[AddComponentMenu("Hexagon Landmass Generator")]
[RequireComponent(typeof(HexagonGrid))]
public class HexGridLandmassGenerator : MonoBehaviour
{

    public Gradient enviormentRamp = new Gradient();
    [HideInInspector]public int seed = 0;
    [HideInInspector]public GeoGenAlgorithm algorithm = GeoGenAlgorithm.Perlin;


    // ds settings
    [HideInInspector]public int mapSize = 64; // Make sure it's 2^n + 1 for the Diamond-Square algorithm
    [HideInInspector]public float roughness = 0.5f;

    // perlin settings
    [HideInInspector]public float frequency = 0.1f;
    [HideInInspector]public float amplitude = 1.0f;
    [HideInInspector]public float noiseScale = 0.17f;
    [HideInInspector]public int octaves = 4;

    [HideInInspector] public Texture2D heightmap;
 
    HexagonGrid layout;

    public void GeneratePerlin(int seed, float noiseScale, float amplitude, float frequency, int octaves)
    {
        Random.InitState(((seed << 32) >> 16) << 32);

        layout = GetComponent<HexagonGrid>();
        var tiles = layout.grid;
        
        if (tiles == null)
        {
            Debug.Log("<color=red>Tiles have not been Generated!</color>");
            return;
        }

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                // Add some offsets to the position for Perlin noise sampling
                float offsetX = transform.position.x;
                float offsetY = transform.position.z;

                // Initialize the final noise value
                float noiseValue = 0f;

                // Calculate the amplitude and frequency for each octave in the current layer
                float layerFrequency = frequency;

                // Calculate the total amplitude to normalize the final noise value
                float totalAmplitude = 0f;

                for (int octave = 0; octave < octaves; octave++)
                {
                    bool isNegative = Random.Range(0, 2) % 2 == 0;

                    // Sample the Perlin noise at the current octave in the current layer
                    float sample = Mathf.PerlinNoise((x + offsetX) * layerFrequency * noiseScale,
                                                     (y + offsetY) * layerFrequency * (isNegative ? -noiseScale : noiseScale));

                    // Apply the power function to the noise sample to adjust the distribution
                    sample = Mathf.Pow(sample, 2);

                    // Sum up the noise samples with appropriate amplitude
                    noiseValue += sample * (amplitude * amplitude);

                    totalAmplitude += amplitude;
                    layerFrequency *= 2f; // You can adjust this factor to control the octave frequencies
                }

                // Normalize the final noise value for the current layer
                noiseValue /= totalAmplitude;

                // Modify the scale based on the normalized noise value and the height multiplier
                var rend = tiles[x, y].GetComponent<HexRenderer>();
                rend.height = noiseValue * 10;
                rend.GenerateMesh();

                var col = rend.GetComponent<MeshCollider>();
                col.convex = true;
                col.sharedMesh = rend.GetMesh();

                // Apply the modified scale back to the tile

                var material = new Material(Resources.Load<Material>("Materials/Tiles/Hexagon"));
                if(rend.height <= 2)
                {
                    material = new Material(Resources.Load<Material>("Materials/Tiles/Hexagon(Transparent)"));
                };

                var color = enviormentRamp.Evaluate(noiseValue);
                material.SetColor("_Base_Color", color);
                tiles[x, y].GetComponent<MeshRenderer>().material = material;
                
                // Set the pixel color in the debug texture based on the normalized noise value
                heightmap.SetPixel(x, y, color);

                var cell = rend.GetComponent<HexagonCell>();
                cell.Init();

                bool hasChildren = rend.transform.childCount > 0;
                if (hasChildren)
                {
                    // update all children to the surface. know children include but are not limited to.
                    //  * debug text object <is removed by debug_mode=false>
                    //  * cell transform object

                    var childcount = rend.transform.childCount;
                    for (int i = childcount - 1; i >= 0; i--)
                    {
                        var child = rend.transform.GetChild(i).gameObject;
                        var pos = child.transform.position;
                        pos.y = (rend.height) + 1e-4f;
                        child.transform.position = pos;
                    }

                }

            }

        }

    }

    public void GenerateAsDiamondSquare(int seed, int mapSize, float roughness)
    {
        // Set a new random seed for the entire tilemap
        Random.InitState(seed);

        // Generate the heightmap using the Diamond-Square algorithm
        GenerateHeightmap(mapSize, roughness);

        // Calculate the size of the texture based on the number of tiles in the tilemap
        var tiles = layout.grid;
        int textureWidth = tiles.GetLength(0); // You can adjust the scale here based on your requirements
        int textureHeight = tiles.GetLength(1); // You can adjust the scale here based on your requirements

        // Transfer the heightmap values to the hexagonal grid
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                var position = tiles[x, y].transform.position;

                // Get the normalized coordinates within the heightmap range
                float nx = x / (float)(textureWidth - 1);
                float ny = y / (float)(textureHeight - 1);

                // Use bilinear interpolation to get the height value from the heightmap
                float heightValue = BilinearInterpolation(heightmap, nx, ny, textureWidth, textureHeight);

                // Modify the scale based on the normalized height value and the height multiplier
                position.y = heightValue * 10;

                // Apply the modified scale back to the tile
                tiles[x, y].transform.position = position;

                // Set the pixel color in the debug texture based on the normalized height value
                heightmap.SetPixel(x, y, enviormentRamp.Evaluate(heightValue));

                // Apply the modified scale back to the tile

                var material = new Material(tiles[x, y].GetComponent<MeshRenderer>().sharedMaterial);
                var color = enviormentRamp.Evaluate(heightValue);
                material.SetColor("_Base_Color", color);
                tiles[x, y].GetComponent<MeshRenderer>().material = material;
                // Set the pixel color in the debug texture based on the normalized noise value
                heightmap.SetPixel(x, y, color);


            }
        }

        heightmap.Apply();
    }

    private float BilinearInterpolation(Texture2D heightmap, float x, float y, int width, int height)
    {
        // Get the integer coordinates of the four surrounding points
        int x0 = Mathf.FloorToInt(x * (width - 1));
        int y0 = Mathf.FloorToInt(y * (height - 1));

        // Get the fractional part of the coordinates
        float xf = x * (width - 1) - x0;
        float yf = y * (height - 1) - y0;

        // Perform bilinear interpolation
        float v00 = heightmap.GetPixel(x0, y0).grayscale;
        float v10 = heightmap.GetPixel(x0 + 1, y0).grayscale;
        float v01 = heightmap.GetPixel(x0, y0 + 1).grayscale;
        float v11 = heightmap.GetPixel(x0 + 1, y0 + 1).grayscale;

        float ix0 = Mathf.Lerp(v00, v10, xf);
        float ix1 = Mathf.Lerp(v01, v11, xf);
        return Mathf.Lerp(ix0, ix1, yf);
    }

    private void GenerateHeightmap(int size, float roughness)
    {
        // Ensure size is 2^n + 1 for the Diamond-Square algorithm
        int requiredSize = 1;
        int iterations = 0;
        while (requiredSize < size || iterations < 5)
        {
            requiredSize = 1 << iterations;
            iterations++;
        }

        if (requiredSize != size || size < 3)
        {
            Debug.LogError("Map size must be 2^n + 1 for the Diamond-Square algorithm.");
            return;
        }

        heightmap = new Texture2D(size, size);

        // Initialize the corners of the heightmap with random values
        heightmap.SetPixel(0, 0, Random.Range(0f, 1f) * Color.white);
        heightmap.SetPixel(0, size - 1, Random.Range(0f, 1f) * Color.white);
        heightmap.SetPixel(size - 1, 0, Random.Range(0f, 1f) * Color.white);
        heightmap.SetPixel(size - 1, size - 1, Random.Range(0f, 1f) * Color.white);

        int stepSize = size - 1;
        for (int i = 0; i < iterations; i++)
        {
            DiamondSquareStep(stepSize, roughness);
            stepSize /= 2;
            roughness /= 2f;
        }

        heightmap.Apply();
    }

    private void DiamondSquareStep(int stepSize, float roughness)
    {
        int halfStep = stepSize / 2;

        for (int x = 0; x < heightmap.width - 1; x += stepSize)
        {
            for (int y = 0; y < heightmap.height - 1; y += stepSize)
            {
                // Diamond step
                float average = (heightmap.GetPixel(x, y).grayscale + heightmap.GetPixel(x + stepSize, y).grayscale +
                                 heightmap.GetPixel(x, y + stepSize).grayscale + heightmap.GetPixel(x + stepSize, y + stepSize).grayscale) / 4f;

                float offset = Random.Range(-roughness, roughness);
                heightmap.SetPixel(x + halfStep, y + halfStep, new Color(average + offset, average + offset, average + offset));
            }
        }

        for (int x = 0; x < heightmap.width; x += halfStep)
        {
            for (int y = (x + halfStep) % stepSize; y < heightmap.height; y += stepSize)
            {
                // Square step
                float sum = 0f;
                int count = 0;

                if (x >= halfStep)
                {
                    sum += heightmap.GetPixel(x - halfStep, y).grayscale;
                    count++;
                }
                if (x + halfStep < heightmap.width)
                {
                    sum += heightmap.GetPixel(x + halfStep, y).grayscale;
                    count++;
                }
                if (y >= halfStep)
                {
                    sum += heightmap.GetPixel(x, y - halfStep).grayscale;
                    count++;
                }
                if (y + halfStep < heightmap.height)
                {
                    sum += heightmap.GetPixel(x, y + halfStep).grayscale;
                    count++;
                }

                float average = sum / count;
                float offset = Random.Range(-roughness, roughness);
                heightmap.SetPixel(x, y, new Color(average + offset, average + offset, average + offset));
            }
        }
    }


}
