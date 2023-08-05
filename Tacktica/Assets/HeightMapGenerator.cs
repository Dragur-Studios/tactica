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

public class HeightMapGenerator 
{
    public static Dictionary<Vector2Int, float> Perlin(HexWorldGenerator layout, int seed, float heightMultiplier, float scale, int octaves, float persistance, float lacunarity)
    {
        Dictionary<Vector2Int, float> heightmap = new Dictionary<Vector2Int, float>();

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offX = prng.Next(-100000, 100000);
            float offY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offX, offY);
        }
        
        if(scale <= 0)
        {
            scale = 0.001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = Mathf.Lerp(-layout.range, layout.range, 0.5f);
        float halfHeight = Mathf.Lerp(-layout.range, layout.range, 0.5f);

        // GENERATE THE HEIGHTMAP USING PERLIN NOISE
        for (int x = -layout.range; x < layout.range; x++)
        {
            for (int y = -layout.range; y < layout.range; y++)
            {
                // Add some offsets to the position for Perlin noise sampling

                var coord = new Vector2Int(x, y);
                
                float height = 0.0f;
                float amplitude = 1.0f;
                float frequency = 1.0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = ((x -halfWidth) / scale + octaveOffsets[i].x) * frequency;
                    float sampleY = ((y - halfHeight) / scale - octaveOffsets[i].y) * frequency;

                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    height += noiseValue * amplitude;
                    
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(height > maxNoiseHeight)
                {
                    maxNoiseHeight = height;
                }
                else if(height < minNoiseHeight)
                {
                    minNoiseHeight = height;
                }

             
                heightmap[coord] = height;   
            }

        }

        // NORMALIZE HEIGHTMAP
        for (int x = -layout.range; x < layout.range; x++)
        {
            for (int y = -layout.range; y < layout.range; y++)
            {
                var coord = new Vector2Int(x, y);
                
                heightmap[coord] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightmap[coord]);
            }
        }


        // RETURN HEIGHTMAP <Vec2Int, Float> where float range<0, 1>
        return heightmap;

    }

    public static Dictionary<Vector2Int, float> DiamondSquare(HexWorldGenerator layout, int seed, int mapSize, float roughness)
    {
        Dictionary<Vector2Int, float> heightmap = new Dictionary<Vector2Int, float>();
        
        var halfMapSize = mapSize / 2;

        int maxHeight = 100;
        int minHeight = -100;

        for (int x = -halfMapSize; x < halfMapSize; x++)
        {
            for (int y = -halfMapSize; y < halfMapSize; y++)
            {
                var coord = new Vector2Int(x, y);
                heightmap.Add(coord, 0);
            } 
        }

        System.Random prng = new System.Random(seed);

        // first set four corners to random heights
        var bl = new Vector2Int(-halfMapSize, -halfMapSize);
        var tl = new Vector2Int(-halfMapSize, halfMapSize);
        var tr = new Vector2Int(halfMapSize, halfMapSize);
        var br = new Vector2Int(halfMapSize, -halfMapSize);

        heightmap[bl] = prng.Next(minHeight, maxHeight);
        heightmap[br] = prng.Next(minHeight, maxHeight);
        heightmap[tl] = prng.Next(minHeight, maxHeight);
        heightmap[tr] = prng.Next(minHeight, maxHeight);

        var chunk_size = mapSize - 1;

        while(chunk_size > 1)
        {
            var half = chunk_size / 2;

            // SQUARE STEP 

            for (int x = -halfMapSize; x < halfMapSize; x+= chunk_size)
            {
                for (int y = -halfMapSize; y < halfMapSize; y += chunk_size)
                {
                    var cur = new Vector2Int(x + half, y + half);


                    float sum = 0;
                    float val = 0;

                    if (heightmap.TryGetValue(new Vector2Int(x, y), out val))
                    {
                        sum += val;
                    }
                    if(heightmap.TryGetValue(new Vector2Int(x + chunk_size, y), out val))
                    {
                        sum += val;
                    }
                    if (heightmap.TryGetValue(new Vector2Int(x, y + chunk_size), out val))
                    {
                        sum += val;
                    }
                    if (heightmap.TryGetValue(new Vector2Int(x + chunk_size, y + chunk_size), out val))
                    {
                        sum += val;
                    }

                    var r = prng.Next(minHeight, maxHeight);
                    
                    var value = (sum / 4) + r ;
                    
                    heightmap[cur] = value;
                }
            }

            // DIAMOND STEP

            for (int y = -halfMapSize; y < halfMapSize; y += half)
            {
                for (int x = (y+half) % chunk_size; x < halfMapSize; x += chunk_size)
                {
                    var cur = new Vector2Int(x, y);
                    // the current value doesnt exist within the 
                    
                    if(!heightmap.ContainsKey(cur))
                    {
                        continue;
                    }

                    float sum = 0;
                    int count = 0;
                    
                    float val = 0.0f;

                    if(heightmap.TryGetValue(new Vector2Int(x, y - half), out val))
                    {
                        sum += val;
                        count++;
                    }
                    if (heightmap.TryGetValue(new Vector2Int(x - half, y), out val))
                    {
                        sum += val;
                        count++;
                    }
                    if (heightmap.TryGetValue(new Vector2Int(x + half, y), out val))
                    {
                        sum += val;
                        count++;
                    }
                    if (heightmap.TryGetValue(new Vector2Int(x, y + half), out val))
                    {
                        sum += val;
                        count++;
                    }

                    var r = prng.Next(minHeight, maxHeight);

                    var value = (sum / count) + r;

                    heightmap[cur] = value;

                }
            }

            chunk_size /= 2;
            roughness /= 2;
        }


        return heightmap;
    }




}
