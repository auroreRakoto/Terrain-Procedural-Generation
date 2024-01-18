using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap
{
    BiomeParameters biomeParams;
    public float[,] Values { get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }
    public int seed;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    public NoiseMap(int width, int height, BiomeParameters biomeParams)
    {
        this.width = width;
        this.height = height;
        this.biomeParams = biomeParams;
        this.seed = biomeParams.seed;
        this.scale = biomeParams.scale;
        this.octaves = biomeParams.octaves;
        this.persistance = 0;
        Values = new float[width, height];
    }

    public NoiseMap GenerateNoiseMap(float scale, float OffsetX, float OffsetY)
    {
        NoiseMap noiseMap = new NoiseMap(this.width, this.height, this.biomeParams);
        noiseMap.GeneratePerlinNoise(scale, offset.x, offset.y);

        return noiseMap;
    }

    public void GeneratePerlinNoise(float scale, float OffsetX, float OffsetY)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                Values[x, y] = noiseHeight;
                //Debug.Log("in Noise Map : " + noiseHeight);


            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Values[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, Values[x, y]);
            }
        }
    }
}
