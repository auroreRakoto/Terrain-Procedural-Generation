using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(int size, BiomManager biomManager, VoronoiDiagram voronoiDiagram)
    {
        // pour recuperer les centres des cellules
        List<Vector2> cellsCenter = new List<Vector2>();
        foreach (var cell in voronoiDiagram.Cells)
        {
            cellsCenter.Add(cell.Center);
        }
        // pour centrer les points
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        MeshData meshData = new MeshData(size);
        int vertexIndex = 0;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // to show the center
                /*bool isElevated = false;
                Vector2 currentVector = new Vector2(x, y);

                foreach (Vector2 point in cellCenter)
                {
                    // Arrondissez les coordonnées du point pour la comparaison
                    int roundedPointX = Mathf.RoundToInt(point.x);
                    int roundedPointY = Mathf.RoundToInt(point.y);

                    if (roundedPointX == x && roundedPointY == y)
                    {
                        isElevated = true;
                        Debug.Log("in");
                        break;
                    }
                }*/
                //meshData.vertices[vertexIndex] = isElevated ? new Vector3(topLeftX + x, 10, topLeftZ - y) : new Vector3(topLeftX + x, 0, topLeftZ - y);

                Vector2 currentVector = new Vector2(x, y);

                // bioms without interpolations
                /*VoronoiCell closestCell = voronoiDiagram.FindCellContainingPoint(voronoiDiagram.Cells, currentVector);
                BiomeType biomeType = closestCell.Biome;
                BiomeParameters biomeParams = biomManager.GetBiomeParameters(biomeType);

                float noiseValue = biomeParams.noiseMap.Values[x, y] * biomeParams.scale;*/
                float noiseValue;
                float searchRadius = 20f;
                List<VoronoiCell> nearbyCells = voronoiDiagram.FindNearbyCells(voronoiDiagram.Cells, currentVector, searchRadius);
                /*if (nearbyCells == null)
                {
                    Debug.Log("cells issue");
                }*/
                /*foreach (var cell in nearbyCells)
                {
                    Debug.Log("cell");
                }*/
                BiomeParameters blendedParams = null;
                blendedParams = biomManager.BlendBiomeParameters(nearbyCells, currentVector, biomManager);

/*                if (blendedParams != null)
                    blendedParams = null;*/

                if (blendedParams != null)
                {
                    Debug.Log("interpolated");
                    int mapX = Mathf.Clamp(x, 0, blendedParams.noiseMap.Width - 1);
                    int mapY = Mathf.Clamp(y, 0, blendedParams.noiseMap.Height - 1);

                    noiseValue = blendedParams.scale * blendedParams.noiseMap.Values[mapX, mapY];
                }
                else
                {
                    VoronoiCell closestCell = voronoiDiagram.FindCellContainingPoint(voronoiDiagram.Cells, currentVector);

                    BiomeType biomeType = closestCell.Biome;
                    BiomeParameters biomeParams = biomManager.GetBiomeParameters(biomeType);

                    noiseValue = biomeParams.noiseMap.Values[x, y] * biomeParams.scale;
                }
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, noiseValue, topLeftZ - y);


                meshData.uvs[vertexIndex] = new Vector2(x / (float)size, y / (float)size);

                if (x < size - 1 && y < size - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshSize)
    {
        vertices = new Vector3[meshSize * meshSize];
        uvs = new Vector2[meshSize * meshSize];
        triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
