using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
    public float[,] Values { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public NoiseMap(int width, int height)
    {
        Width = width;
        Height = height;
        Values = new float[width, height];
    }

    public NoiseMap GenerateNoiseMap(float scale, float offsetX, float offsetY)
    {
        NoiseMap noiseMap = new NoiseMap(Width, Height);
        noiseMap.GeneratePerlinNoise(scale, offsetX, offsetY);

        // Optionally adjust the noise map based on cell properties here

        return noiseMap;
    }

    public void GeneratePerlinNoise(float scale, float offsetX, float offsetY)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float sampleX = x / scale + offsetX;
                float sampleY = y / scale + offsetY;
                float noise = Mathf.PerlinNoise(sampleX, sampleY);
                Values[x, y] = noise;
            }
        }
    }
}
