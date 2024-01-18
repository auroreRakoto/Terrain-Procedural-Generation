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

        List<Vector2> points = GenerateRandomPoints2D(numberOfPoints, rangeX, rangeY);
        float borderThreshold = size.x / 3f;

        foreach (Vector2 point in points)
        {
            VoronoiCell newCell = new VoronoiCell
            {
                Center = point,
                IsBorderCell = (point.x <= rangeX.x + borderThreshold) ||
                           (point.x >= rangeX.y - borderThreshold) ||
                           (point.y <= rangeY.x + borderThreshold) ||
                           (point.y >= rangeY.y - borderThreshold)
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

}
