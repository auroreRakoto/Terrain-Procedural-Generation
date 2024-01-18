using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoronoiDiagram
{
    public List<VoronoiCell> Cells { get; private set; }

    public VoronoiDiagram()
    {
        Cells = new List<VoronoiCell>();
    }

    public static List<Vector2> GenerateRandomPoints2D(int numPoints, Vector2 rangeX, Vector2 rangeY)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < numPoints; i++)
        {
            float x = UnityEngine.Random.Range(rangeX.x, rangeX.y);
            float y = UnityEngine.Random.Range(rangeY.x, rangeY.y);
            points.Add(new Vector2(x, y));
        }
        return points;
    }

    public void GenerateDiagram(Vector2 size, int numberOfPoints)
    {
        Vector2 rangeX = new Vector2(0, size.x);
        Vector2 rangeY = new Vector2(0, size.y);

        // Utilisez la méthode GenerateRandomPoints2D pour créer des points aléatoires
        List<Vector2> points = GenerateRandomPoints2D(numberOfPoints, rangeX, rangeY);

        // Créez des cellules de Voronoi pour chaque point généré
        foreach (Vector2 point in points)
        {
            VoronoiCell newCell = new VoronoiCell
            {
                Center = point,
                // Initialisez d'autres propriétés de la cellule si nécessaire
            };
            Cells.Add(newCell);
        }

        // Ici, vous pourriez ajouter la logique pour déterminer les voisins de chaque cellule
        // et tout autre traitement nécessaire pour compléter le diagramme de Voronoi
    }

    public VoronoiCell FindCellContainingPoint(List<VoronoiCell> cells, Vector2 point)
    {
        VoronoiCell closestCell = null;
        float minDistanceSquared = float.MaxValue;

        foreach (var cell in cells)
        {
            float distanceSquared = (cell.Center - point).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                closestCell = cell;
            }
        }

        return closestCell;
    }

    public List<VoronoiCell> FindNearbyCells(List<VoronoiCell> allCells, Vector2 point, float searchRadius)
    {
        List<VoronoiCell> nearbyCells = new List<VoronoiCell>();
        foreach (var cell in allCells)
        {
            float distance = Vector2.Distance(cell.Center, point);
            if (distance <= searchRadius)
            {
                nearbyCells.Add(cell);
            }
        }
        return nearbyCells;
    }


    /*
        public static Texture2D GenerateVoronoiTexture(int mapSize, List<Vector2> cells)
        {
            Texture2D texture = new Texture2D(mapSize, mapSize);

            // Initialize each pixel to a default color
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    texture.SetPixel(i, j, Color.white);
                }
            }

            // Assign each pixel to the closest point and color the cell
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Vector2 pixelPos = new Vector2(i, j);
                    int closestPointIndex = FindClosestPointIndex(pixelPos, cells);
                    Color cellColor = GetColorForCell(closestPointIndex, cells.Count);
                    texture.SetPixel(i, j, cellColor);
                }
            }

            texture.Apply();
            return texture;
        }

        private static int FindClosestPointIndex(Vector2 pixelPos, List<Vector2> points)
        {
            int closestPointIndex = 0;
            float minDistance = float.MaxValue;

            for (int i = 0; i < points.Count; i++)
            {
                float distance = Vector2.Distance(pixelPos, points[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPointIndex = i;
                }
            }

            return closestPointIndex;
        }

        private static Color GetColorForCell(int index, int totalPoints)
        {
            // Generate a unique color for each cell based on its index
            float hue = (float)index / totalPoints;
            return Color.HSVToRGB(hue, 0.8f, 0.8f);
        }

        public static List<Vector2> GenerateRandomPoints2D(int numPoints, Vector2 rangeX, Vector2 rangeY)
        {
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < numPoints; i++)
            {
                float x = UnityEngine.Random.Range(rangeX.x, rangeX.y);
                float y = UnityEngine.Random.Range(rangeY.x, rangeY.y);
                points.Add(new Vector2(x, y));
            }
            return points;
        }

        public static List<VoronoiCell> GenerateIslandPoints(int numPoints, Vector2 rangeX, Vector2 rangeY)
        {
            Vector2 center = new Vector2((rangeX.y - rangeX.x) / 2, (rangeY.y - rangeY.x) / 2);
            List<VoronoiCell> cells = new List<VoronoiCell>();


            for (int i = 0; i < numPoints; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * Mathf.Min(center.x / 2, center.y / 2);
                randomPoint += center;

                VoronoiCell cell = new VoronoiCell();
                cell.Center = randomPoint;
                cells.Add(cell);
            }
            //DetermineBiome(cells, rangeX, rangeY);

            return cells;
        }

        private static BiomeType DetermineBiome(List<VoronoiCell> cells, Vector2 rangeX, Vector2 rangeY)
        {
            // Logique pour déterminer si le point est proche d'un bord
            if (IsNearEdge(point, rangeX, rangeY))
            {
                return BiomeType.Sea;
            }
            else
            {
                return (BiomeType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(BiomeType)).Length);
            }
        }

        private static bool IsNearEdge(Vector2 point, Vector2 rangeX, Vector2 rangeY)
        {
            float edgeThreshold = 10; // Seuil pour considérer qu'un point est proche du bord

            bool nearLeftEdge = point.x <= rangeX.x + edgeThreshold;
            bool nearRightEdge = point.x >= rangeX.y - edgeThreshold;
            bool nearTopEdge = point.y <= rangeY.x + edgeThreshold;
            bool nearBottomEdge = point.y >= rangeY.y - edgeThreshold;

            return nearLeftEdge || nearRightEdge || nearTopEdge || nearBottomEdge;
        }



        public static List<Vector2> GetAdjustedVoronoiPoints(Vector2 chunkPosition, int numPoints, Vector2 rangeX, Vector2 rangeY, Dictionary<Vector2, List<Vector2>> existingChunks)
        {
            List<Vector2> adjustedPoints = new List<Vector2>();

            // Logique pour récupérer les points des chunks adjacents
            foreach (var adjacentChunkPos in GetAdjacentChunkPositions(chunkPosition))
            {
                if (existingChunks.ContainsKey(adjacentChunkPos))
                {
                    List<Vector2> sharedPoints = GetSharedPoints(chunkPosition, adjacentChunkPos, existingChunks[adjacentChunkPos]);
                    adjustedPoints.AddRange(sharedPoints);
                }
            }

            // Générer de nouveaux points pour le chunk actuel
            List<Vector2> newPoints = GenerateRandomPoints2D(numPoints, rangeX, rangeY);

            // Fusionner avec les points des chunks adjacents
            adjustedPoints.AddRange(newPoints);

            return adjustedPoints;
        }

        // Vous devrez implémenter GetAdjacentChunkPositions et GetSharedPoints
        private List<Vector2> GetAdjacentChunkPositions(Vector2 chunkPosition)
        {
            List<Vector2> adjacentPositions = new List<Vector2>();
            adjacentPositions.Add(new Vector2(chunkPosition.x + 1, chunkPosition.y)); // Droite
            adjacentPositions.Add(new Vector2(chunkPosition.x - 1, chunkPosition.y)); // Gauche
            adjacentPositions.Add(new Vector2(chunkPosition.x, chunkPosition.y + 1)); // Haut
            adjacentPositions.Add(new Vector2(chunkPosition.x, chunkPosition.y - 1)); // Bas
            return adjacentPositions;
        }*/


}
