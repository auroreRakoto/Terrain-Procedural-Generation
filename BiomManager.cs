using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum BiomeType
{
    Forest,
    Mountain,
    Desert,
    //Prairie,
    Plain,
    Sea
    //Beach
}

public class BiomeParameters
{
    public int seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public Color biomeColor;
    public NoiseMap noiseMap;
}

public class BiomManager
{
    private Dictionary<BiomeType, BiomeParameters> biomeParameters = new Dictionary<BiomeType, BiomeParameters>();
    Color brownColor = new Color(0.5f, 0.3f, 0.1f);
    Color darkGreenColor = new Color(0.0f, 0.3f, 0.0f);
    Color lightGreenColor = new Color(0.0f, 0.7f, 0.0f);
    public int mapSize;

    public BiomManager(int size)
    {
        int noiseMapWidth = size;
        int noiseMapHeight = size;
        mapSize = size;

        BiomeParameters forestParams = new BiomeParameters
        {
            seed = Random.Range(0, 100),
            scale = 15f,
            octaves = 6,
            persistance = 0.5f,
            lacunarity = 1.8f,
            offset = new Vector2(0, 0),
            biomeColor = darkGreenColor
        };
        forestParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, forestParams).GenerateNoiseMap(forestParams.scale, forestParams.offset.x, forestParams.offset.y);
        biomeParameters[BiomeType.Forest] = forestParams;

        BiomeParameters plainParams = new BiomeParameters
        {
            seed = Random.Range(0, 100),
            scale = 12f,
            octaves = 5,
            persistance = 0.4f,
            lacunarity = 2.1f,
            offset = new Vector2(0, 0),
            biomeColor = new Color(0.6f, 0.8f, 0.4f)
        };
        plainParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, plainParams).GenerateNoiseMap(plainParams.scale, plainParams.offset.x, plainParams.offset.y);
        biomeParameters[BiomeType.Plain] = plainParams;


        // Initialize parameters for Mountain biome
        BiomeParameters mountainParams = new BiomeParameters
        {
            seed = Random.Range(0, 100),
            scale = 35f,
            octaves = 4,
            persistance = 0.5f,
            lacunarity = 1.8f,
            offset = new Vector2(0, 0),
            biomeColor = brownColor
        };
        mountainParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, mountainParams).GenerateNoiseMap(mountainParams.scale, mountainParams.offset.x, mountainParams.offset.y);
        biomeParameters[BiomeType.Mountain] = mountainParams;
        Debug.Log("noise in biom : " + mountainParams.noiseMap.Values);

        // Initialize parameters for Desert biome
        BiomeParameters desertParams = new BiomeParameters
        {
            seed = Random.Range(0, 100),
            scale = 8f,
            octaves = 4,
            persistance = 0.2f,
            lacunarity = 1.9f,
            offset = new Vector2(0, 0),
            biomeColor = Color.yellow
        };
        desertParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, desertParams).GenerateNoiseMap(desertParams.scale, desertParams.offset.x, desertParams.offset.y);
        biomeParameters[BiomeType.Desert] = desertParams;

        // Initialize parameters for Sea biome
        BiomeParameters seaParams = new BiomeParameters
        {
            seed = Random.Range(0, 100),
            scale = 5f,
            octaves = 3,
            persistance = 0.1f,
            lacunarity = 1.7f,
            offset = new Vector2(0, 0),
            biomeColor = Color.blue
        };
        seaParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, seaParams).GenerateNoiseMap(seaParams.scale, seaParams.offset.x, seaParams.offset.y);
        biomeParameters[BiomeType.Sea] = seaParams;
        /*foreach (var biomeParam in biomeParameters.Values)
        {
            biomeParam.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight, biomeParam)
                .GenerateNoiseMap(biomeParam.scale, biomeParam.offset.x, biomeParam.offset.y);
        }*/

    }

    public void AssignBiomeToCells(List<VoronoiCell> cells)
    {
        // First, assign biomes randomly or based on specific criteria
        foreach (var cell in cells)
        {
            //float circleRadius = 50;

            //float distanceToCircleCenter = Vector2.Distance(cell.Center, new Vector2(mapSize/2, mapSize/2));
            //if (distanceToCircleCenter > circleRadius)
            if (cell.IsBorderCell)
            {
                cell.Biome = BiomeType.Sea;
            }
            else
                cell.Biome = (BiomeType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(BiomeType)).Length - 1);
            //cell.Biome = randomBiomeType;
        }

        // Then, adjust the biomes based on neighbors
        /*foreach (var cell in cells)
        {
            // Check if the current cell is a Mountain
            if (cell.Biome == BiomeType.Mountain)
            {
                // Skip this cell, as Mountains should not change their biome
                continue;
            }

            // Determine neighbors of the cell
            List<VoronoiCell> neighbors = GetNeighbors(cell, cells);

            // Check if any neighbor is a Mountain
            if (neighbors.Any(neighbor => neighbor.Biome == BiomeType.Sea))
            {
                // Set this cell to Forest if a neighbor is Mountain
                Debug.Log("near sea");
                cell.Biome = BiomeType.Desert;
            }
        }*/
    }

    private List<VoronoiCell> GetNeighbors(VoronoiCell cell, List<VoronoiCell> cells)
    {
        List<VoronoiCell> neighbors = new List<VoronoiCell>();

        foreach (var otherCell in cells)
        {
            // Skip the current cell itself
            if (otherCell == cell)
                continue;

            // Calculate the distance between the centers of the two cells
            float distance = Vector2.Distance(cell.Center, otherCell.Center);

            // You can define a threshold distance to consider cells as neighbors
            float thresholdDistance = 10.0f; // Adjust this value as needed

            if (distance < thresholdDistance)
            {
                // If the distance is less than the threshold, consider it a neighbor
                neighbors.Add(otherCell);
            }
        }

        return neighbors;
    }

    public BiomeParameters BlendBiomeParameters(List<VoronoiCell> nearbyCells, Vector2 point, BiomManager biomManager)
    {
        if (nearbyCells == null)
        {
            Debug.LogError("BlendBiomeParameters: nearbyCells is null");
            return null; // Or handle this case appropriately
        }
        

        BiomeParameters blendedParams = new BiomeParameters();
        float totalWeight = 0;
        
        // Initialize accumulators for each parameter
        float totalScale = 0;
        float totalLacunarity = 0;
        int totalOctave = 0;
        float totalNoiseValue = 0; // Accumulator for noise

        foreach (var cell in nearbyCells)
        {
            float distance = Vector2.Distance(cell.Center, point);
            float weight = 1 / Mathf.Pow(distance, 2);

            BiomeParameters cellParams = biomManager.GetBiomeParameters(cell.Biome);

            totalScale += cellParams.scale * weight;
            totalLacunarity += cellParams.lacunarity * weight;
            totalOctave += Mathf.RoundToInt(cellParams.octaves * weight);

            
            int mapX = Mathf.Clamp((int)point.x, 0, cellParams.noiseMap.width - 1);
            int mapY = Mathf.Clamp((int)point.y, 0, cellParams.noiseMap.height - 1);

            float noiseValue = cellParams.noiseMap.Values[mapX, mapY] * weight;
            totalNoiseValue += noiseValue * weight;

            totalWeight += weight;
        }
        blendedParams.scale = totalScale / totalWeight;
        blendedParams.lacunarity = totalLacunarity / totalWeight;
        blendedParams.octaves = Mathf.RoundToInt((float)totalOctave / totalWeight);
        blendedParams.noiseMap = new NoiseMap(mapSize, mapSize, blendedParams).GenerateNoiseMap(blendedParams.scale, blendedParams.offset.x, blendedParams.offset.y);
        return blendedParams;
    }

    public BiomeParameters GetBiomeParameters(BiomeType biomeType)
    {
        // Get the custom parameters for a specific biome type
        if (biomeParameters.ContainsKey(biomeType))
        {
            return biomeParameters[biomeType];
        }
        else
        {
            // Return default parameters or handle the case when the biome type is not found
            return new BiomeParameters();
        }
    }
}

