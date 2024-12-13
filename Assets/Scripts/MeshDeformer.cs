using System.IO;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    private MeshFilter surfaceMeshFilter;
    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;
    public float deformationStrength = 0.1f; // How deep the indentation is
    public float deformationRadius = 1f; // How wide the deformation should be
    public GameObject ball;

    public string filePathBall = "Assets/ball_data.txt"; // Path to save the mesh and ball data
    public string filePathSurface = "Assets/surface_data.txt"; // Path to save the mesh and ball data

    private void Start()
    {
        surfaceMeshFilter = GetComponent<MeshFilter>();
        originalVertices = surfaceMeshFilter.mesh.vertices;
        deformedVertices = (Vector3[])originalVertices.Clone();

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 impactPoint = collision.GetContact(0).point;

        // Deform surface
        DeformSurface(impactPoint);

        // Optionally, draw the deformation
        surfaceMeshFilter.mesh.vertices = deformedVertices;
        surfaceMeshFilter.mesh.RecalculateNormals();
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
            writer.WriteLine("Deformed Surface Mesh Vertices:");
            foreach (Vector3 vertex in deformedVertices)
            {
                writer.WriteLine(vertex.x + ", " + vertex.y + ", " + vertex.z);
            }
        }
    }

    private void Update()
    {
        //// Check if the ball is colliding and deform the surface
        //// For this, we'll use raycast or collision detection
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        //{
        //    Vector3 impactPoint = hit.point;

        //    // Deform surface
        //    DeformSurface(impactPoint);

        //    // Optionally, draw the deformation
        //    surfaceMeshFilter.mesh.vertices = deformedVertices;
        //    surfaceMeshFilter.mesh.RecalculateNormals();
        //}
        //else
        //{
        //    // If no collision, restore the surface
        //    RestoreSurface();
        //}
        // Save the mesh and ball data after deformation
        SaveMeshAndBallDataToFile();
    }

    private void DeformSurface(Vector3 impactPoint)
    {
        int cnt = 0;
        float total = 0;
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            //if (i % 5 == 0)
            //{
            //    Debug.DrawLine(deformedVertices[i], impactPoint, Color.red, 200);
            //}
            // Calculate the distance from the vertex to the impact point
            Vector3 worldVertexPosition = transform.TransformPoint(deformedVertices[i]);
            float distance = Vector3.Distance(worldVertexPosition, impactPoint);
            total += distance;
            // If within the deformation radius, move the vertex downward
            if (distance < deformationRadius)
            {
                cnt++;
                Vector3 before = deformedVertices[i];
                float deformationAmount = (1 - (distance / deformationRadius)) * deformationStrength;
                // deformedVertices[i] -= deformationAmount * (deformedVertices[i] - impactPoint);
                deformedVertices[i].y -= deformationAmount;               

            }
        }
        Debug.Log("Changed " + cnt + " out of " + deformedVertices.Length);
        Debug.Log("Average distance: " + total/deformedVertices.Length);
    }

    //private void RestoreSurface()
    //{
    //    // Smoothly restore the vertices back to the original position (lerp)
    //    for (int i = 0; i < deformedVertices.Length; i++)
    //    {
    //        deformedVertices[i] = Vector3.Lerp(deformedVertices[i], originalVertices[i], Time.deltaTime);
    //    }
    //}

    
}
