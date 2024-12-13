using UnityEngine;

public class ManualDeformation : MonoBehaviour
{
    public MeshFilter surfaceMeshFilter;
    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;
    private float deformationStrength = 0.1f; // How deep the indentation is
    private float deformationRadius = 1f; // How wide the deformation should be

    private void Start()
    {
        originalVertices = surfaceMeshFilter.mesh.vertices;
        deformedVertices = (Vector3[])originalVertices.Clone();
    }

    private void Update()
    {
        // Check if the ball is colliding and deform the surface
        // For this, we'll use raycast or collision detection
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        {
            Vector3 impactPoint = hit.point;

            // Deform surface
            DeformSurface(impactPoint);

            // Optionally, draw the deformation
            surfaceMeshFilter.mesh.vertices = deformedVertices;
            surfaceMeshFilter.mesh.RecalculateNormals();
        }
        else
        {
            // If no collision, restore the surface
            RestoreSurface();
        }
    }

    private void DeformSurface(Vector3 impactPoint)
    {
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            // Calculate the distance from the vertex to the impact point
            float distance = Vector3.Distance(deformedVertices[i], impactPoint);

            // If within the deformation radius, move the vertex downward
            if (distance < deformationRadius)
            {
                float deformationAmount = (1 - (distance / deformationRadius)) * deformationStrength;
                deformedVertices[i].z -= deformationAmount;
            }
        }
    }

    private void RestoreSurface()
    {
        // Smoothly restore the vertices back to the original position (lerp)
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            deformedVertices[i] = Vector3.Lerp(deformedVertices[i], originalVertices[i], Time.deltaTime);
        }
    }
}
