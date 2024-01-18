using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator
{
    VoronoiDiagram voronoiDiagram;
    BiomManager biomManager;

    public TextureGenerator(VoronoiDiagram voronoiDiagram, BiomManager biomManager)
    {
        this.voronoiDiagram = voronoiDiagram;
        this.biomManager = biomManager;
    }

    public Texture2D GenerateTexture(int width, int height, List<VoronoiCell> cells)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                VoronoiCell cell = voronoiDiagram.FindCellContainingPoint(cells, new Vector2(x, y));
                BiomeType biomeType = cell.Biome;
                BiomeParameters parameters = biomManager.GetBiomeParameters(biomeType);
                Color color = parameters.biomeColor;
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
