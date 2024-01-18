```
# Terrain Generation with Biome Interpolation

## Description

A Unity project for procedural terrain generation using Voronoi diagrams and biome interpolation for dynamic landscapes.

## Setup

To set up the project in Unity, follow these steps:

1. **Clone the Repository:**
   ```bash
   git clone Repository-URL
   ```

2. **Open the Project in Unity:**
   - Launch Unity Hub.
   - Click on 'Add' and navigate to the cloned project directory.
   - Select and open the project.

3. **Scene Setup:**
   - In your Unity project, open the main scene where you want to generate the terrain.
   - Create an empty GameObject in the scene.
   - Attach the `MainController` script to the empty GameObject.

4. **Mesh Filter and Mesh Renderer:**
   - With the empty GameObject selected, add a `Mesh Filter` and a `Mesh Renderer` component to it.
   - You can assign a material to the Mesh Renderer for better visualization.

## Usage

1. **Adjusting Parameters:**
   - Select the GameObject with the `MainController` script.
   - In the Inspector window, you can adjust various parameters such as the size of the terrain, the number of points for the Voronoi diagram, and other biome-related settings.

2. **Generating the Terrain:**
   - Press the 'Play' button in Unity.
   - The script will automatically generate the terrain based on the provided settings and render it using the attached Mesh Filter and Mesh Renderer.

3. **Experimenting with Biomes:**
   - Modify biome parameters in the `BiomManager` script to see how different settings affect the terrain.
   - Experiment with different noise parameters, scales, and biome types to achieve the desired terrain look.

## Contributing

Feel free to fork the project and submit pull requests. You can also open issues for any bugs found or features you think would enhance the project.
```
