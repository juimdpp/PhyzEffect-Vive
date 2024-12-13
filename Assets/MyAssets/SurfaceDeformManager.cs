using UnityEngine;
using System.IO;

public class SurfaceDeformManager : MonoBehaviour
{
    public Shader surfaceShader;             // Shader used for vertex coloring
    public GameObject ball;                 // The ball object causing the collision
    private Mesh mesh;                      // The mesh of the surface
    private Vector3[] vertices;             // Vertices of the mesh
    private Color[] vertexColors;           // Array to hold the colors of the vertices
    public float deformationRadius = 2.0f;  // How far from the impact point to consider "near"
    public float deformationStrength = 0.1f; // How strong the deformation is

    private Material surfaceMaterial;
    public string filePathBall = "Assets/ball_data.txt"; // Path to save the mesh and ball data
    public string filePathSurface = "Assets/surface_data.txt"; // Path to save the mesh and ball data


    void Start()
    {
        // Get the mesh of the surface and initialize the color array
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        vertexColors = new Color[vertices.Length];

        // Set initial vertex color to white
        for (int i = 0; i < vertexColors.Length; i++)
        {
            vertexColors[i] = Color.white;
        }

        // Create a material using the shader and apply it to the surface
        surfaceMaterial = new Material(surfaceShader);
        GetComponent<Renderer>().material = surfaceMaterial;

        // Initially set the mesh colors to the starting colors
        mesh.colors = vertexColors;

        deformationRadius = CalculateDeformationRadius();

        // Clear the file at the start to avoid appending to old data
        File.WriteAllText(filePathBall, string.Empty);
        File.WriteAllText(filePathSurface, string.Empty);

    }

    void Update()
    {
        // Perform collision check (Raycast or other method to detect the ball's impact)
        if (ball != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(ball.transform.position, Vector3.down, out hit))
            {
                Vector3 impactPoint = hit.point;
                ApplyDeformation(impactPoint);
            }
        }
        // Save the mesh and ball data after deformation
        SaveMeshAndBallDataToFile();
    }

    // Apply deformation based on the proximity of the collision point
    void ApplyDeformation(Vector3 impactPoint)
    {
        // Loop through each vertex in the mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            // Calculate the distance between the vertex and the impact point
            Vector3 worldVertexPosition = transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(worldVertexPosition, impactPoint);

            // If the vertex is within the deformation radius, modify its color
            if (distance <= deformationRadius)
            {
                // Apply deformation strength and set the color (stronger deformation = redder color)
                float deformationFactor = 1.0f - (distance / deformationRadius);
                deformationFactor *= deformationStrength;

                // Modify the Z position for deformation (can be replaced with other deformation logic)
                vertices[i].z -= deformationFactor;

                // Assign a random color based on the proximity
                vertexColors[i] = new Color(Random.value, Random.value, Random.value); // Random color for each vertex near the impact
            }
        }

        // Update the mesh with the modified vertices and colors
        mesh.vertices = vertices;
        mesh.colors = vertexColors;
    }

    // Calculate the deformation radius based on the mesh's bounding box size
    float CalculateDeformationRadius()
    {
        Collider boxCollider = ball.GetComponent<Collider>();
        // Use the bounding box's size to determine the deformation radius
        if (boxCollider != null)
        {
            // Calculate the radius from the collider's size
            float radius = boxCollider.bounds.extents.magnitude; // Get half the diagonal size
            return radius;  // Use the radius as a starting point for deformation
        }

        // If no collider is present, fall back to a fixed value
        return 1.0f;  // Default radius if no collider is found
    }

    private void SaveMeshAndBallDataToFile()
    {
        // Open or create the file and write the data
        using (StreamWriter writer = new StreamWriter(filePathBall, true)) // 'true' to append to the file
        {
            // Write the current frame's data
            writer.WriteLine($"Frame: {Time.frameCount}");
            // Write the ball position
            writer.WriteLine(ball.GetComponent<Transform>().position);
        }

        // Open or create the file and write the data
        using (StreamWriter writer = new StreamWriter(filePathSurface, true)) // 'true' to append to the file
        {
            // Write the current frame's data
            writer.WriteLine($"Frame: {Time.frameCount}");
            // Write the surface mesh vertices
            foreach (Vector3 vertex in vertices)
            {
                Vector3 temp = transform.TransformPoint(vertex);
                writer.WriteLine(temp.x + ", " + temp.y + ", " + temp.z);
            }
        }
    }
}
