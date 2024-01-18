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
    Sea,
    Plain
    //Beach
}

public class BiomeParameters
{
    public float lacunarity;
    public int octave;
    public float scale;
    public float offsetX;
    public float offsetY;
    public Color biomeColor;
    public NoiseMap noiseMap;
}

public class BiomManager
{
    private Dictionary<BiomeType, BiomeParameters> biomeParameters = new Dictionary<BiomeType, BiomeParameters>();

    public BiomManager()
    {
        Color brownColor = new Color(0.5f, 0.3f, 0.1f);
        Color darkGreenColor = new Color(0.0f, 0.3f, 0.0f);
        Color lightGreenColor = new Color(0.0f, 0.7f, 0.0f);
        int noiseMapWidth = 100;
        int noiseMapHeight = 100;

        BiomeParameters forestParams = new BiomeParameters
        {
            lacunarity = 2.0f,
            octave = 4,
            scale = 12f,
            biomeColor = darkGreenColor,
            offsetX = 0,
            offsetY = 0
        };
        forestParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight).GenerateNoiseMap(forestParams.scale, forestParams.offsetX, forestParams.offsetY);
        biomeParameters[BiomeType.Forest] = forestParams;

        BiomeParameters plainParams = new BiomeParameters
        {
            lacunarity = 2.0f,
            octave = 4,
            scale = 8f,
            biomeColor = lightGreenColor,
            offsetX = 0,
            offsetY = 0
        };
        plainParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight).GenerateNoiseMap(plainParams.scale, plainParams.offsetX, plainParams.offsetY);
        biomeParameters[BiomeType.Plain] = plainParams;


        // Initialize parameters for Mountain biome
        BiomeParameters mountainParams = new BiomeParameters
        {
            lacunarity = 2.5f,
            octave = 6,
            scale = 20f,
            biomeColor = brownColor, // Assuming you've defined brownColor previously
            offsetX = 0,
            offsetY = 0
        };
        mountainParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight).GenerateNoiseMap(mountainParams.scale, mountainParams.offsetX, mountainParams.offsetY);
        biomeParameters[BiomeType.Mountain] = mountainParams;

        // Initialize parameters for Desert biome
        BiomeParameters desertParams = new BiomeParameters
        {
            lacunarity = 1.5f,
            octave = 3,
            scale = 5f,
            biomeColor = Color.yellow
        };
        desertParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight).GenerateNoiseMap(desertParams.scale, desertParams.offsetX, desertParams.offsetY);
        biomeParameters[BiomeType.Desert] = desertParams;

        // Initialize parameters for Sea biome
        BiomeParameters seaParams = new BiomeParameters
        {
            lacunarity = 1.5f,
            octave = 3,
            scale = 2f,
            biomeColor = Color.blue
        };
        seaParams.noiseMap = new NoiseMap(noiseMapWidth, noiseMapHeight).GenerateNoiseMap(seaParams.scale, seaParams.offsetX, seaParams.offsetY);
        biomeParameters[BiomeType.Sea] = seaParams;

    }

    public void AssignBiomeToCells(List<VoronoiCell> cells)
    {
        // First, assign biomes randomly or based on specific criteria
        foreach (var cell in cells)
        {
            // Randomly select a biome type
            BiomeType randomBiomeType = (BiomeType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(BiomeType)).Length);
            cell.Biome = randomBiomeType;
        }

        // Then, adjust the biomes based on neighbors
        foreach (var cell in cells)
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
        }
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

        if (biomManager == null)
        {
            Debug.LogError("BlendBiomeParameters: biomManager is null");
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

            // Accumulate weighted parameters
            totalScale += cellParams.scale * weight;
            totalLacunarity += cellParams.lacunarity * weight;
            totalOctave += Mathf.RoundToInt(cellParams.octave * weight);


            int mapX = Mathf.Clamp((int)point.x, 0, blendedParams.noiseMap.Width - 1);
            int mapY = Mathf.Clamp((int)point.y, 0, blendedParams.noiseMap.Height - 1);

            // Accumulate weighted noise values
            float noiseValue = cellParams.noiseMap.Values[mapX, mapY] * weight;
            totalNoiseValue += noiseValue * weight;

            totalWeight += weight;
        }

        // Normalize blended parameters
        blendedParams.scale = totalScale / totalWeight;
        blendedParams.lacunarity = totalLacunarity / totalWeight;
        blendedParams.octave = Mathf.RoundToInt((float)totalOctave / totalWeight);
        blendedParams.noiseMap = new NoiseMap(100, 100).GenerateNoiseMap(blendedParams.scale, blendedParams.offsetX, blendedParams.offsetY);

        return blendedParams;
    }


    /*public void InterpolateBiomes(List<VoronoiCell> cells, List<Vector2> samplingPoints)
    {
        foreach (var point in samplingPoints)
        {
            // Find Voronoi cells that surround the sampling point
            List<VoronoiCell> surroundingCells = GetNeighbors(point, cells);

            // Calculate total area for normalization
            float totalArea = surroundingCells.Sum(cell => cell.Area);

            // Interpolate biome parameters based on cell weights
            BiomeParameters interpolatedBiome = new BiomeParameters();
            foreach (var cell in surroundingCells)
            {
                // Calculate the weight for the cell based on its area
                float cellWeight = cell.Area / totalArea;

                // Interpolate biome parameters based on the cell weight
                interpolatedBiome.lacunarity += cell.BiomeParameters.lacunarity * cellWeight;
                interpolatedBiome.octave += cell.BiomeParameters.octave * cellWeight;
                // Interpolate other biome parameters similarly

                // You can also interpolate colors, noise maps, and other properties here
            }

            // Use the interpolatedBiome parameters to apply to the sampling point
            // You can set the biome parameters or perform other actions here
            // For example, you can change the terrain color or texture at this point
        }
    }*/


    public void DetermineBorderCells(List<VoronoiCell> cells)
    {
        // Méthode pour déterminer les cellules frontalières
    }

    public void CalculateInterpolationWeights(List<VoronoiCell> cells)
    {
        // Méthode pour calculer les poids d'interpolation
    }

    public void InterpolateBorders(List<VoronoiCell> cells)
    {
        // Méthode pour appliquer l'interpolation aux bords
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

