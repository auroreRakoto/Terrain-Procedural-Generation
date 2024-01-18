using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MainController : MonoBehaviour
{
    [SerializeField] private static int size = 200;
    public int numberOfPoints = 50;
    public Vector2 rangeX = new Vector2(0, size);
    public Vector2 rangeY = new Vector2(0, size);

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    VoronoiDiagram voronoiDiagram;
    TextureGenerator textureGenerator;
    BiomManager biomManager;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize your VoronoiDiagram
        voronoiDiagram = new VoronoiDiagram();
        biomManager = new BiomManager(size);
        if (biomManager == null)
        {
            Debug.LogError("MainController: biomManager is not initialized");
            return;
        }

        if (voronoiDiagram == null)
        {
            Debug.LogError("MainController: voronoiDiagram is not initialized");
            return;
        }
        // Create an instance of TextureGenerator
        textureGenerator = new TextureGenerator(voronoiDiagram, biomManager);

        // Define the size of your diagram area (example: 100x100 units)
        //Vector2 size = new Vector2(100, 100);
        // Define the number of points/cells you want in your diagram

        // Call the GenerateDiagram method to create the Voronoi diagram
        voronoiDiagram.GenerateDiagram(new Vector2(size, size), numberOfPoints);
        biomManager.AssignBiomeToCells(voronoiDiagram.Cells);

        // After generating the diagram, you can now work with the cells
        // For example, you can pass them to a mesh generator or visualize them
        List<Vector2> centerPoints = new List<Vector2>();
        foreach (var cell in voronoiDiagram.Cells)
        {
            centerPoints.Add(cell.Center);
        }
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(size, biomManager, voronoiDiagram);
        meshFilter.mesh = meshData.CreateMesh();
        meshRenderer.material.mainTexture = textureGenerator.GenerateTexture(size, size, voronoiDiagram.Cells);
    }

    // Update is called once per frame
    void Update()
    {
        //infiniteTerrain.UpdateTerrain();
    }

    public void GenerateIsland()
    {
        // Méthode pour coordonner la génération de l'île
    }
}
