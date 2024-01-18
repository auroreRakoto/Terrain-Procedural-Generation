using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiCell
{
    public Color Color;
    public float[,] NoiseMap;
    public Vector2[] cellPoints;

    public Vector2 Center { get; set; }
    public BiomeType Biome { get; set; }
    public bool IsBorderCell { get; set; }
    //public Dictionary<string, float> InterpolatedFeatures { get; set; }

}
