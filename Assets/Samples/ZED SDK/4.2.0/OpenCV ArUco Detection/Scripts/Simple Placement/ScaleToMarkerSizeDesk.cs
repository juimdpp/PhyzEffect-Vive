﻿#if ZED_OPENCV_FOR_UNITY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scales this object so that a given mesh within the object fits within the bounds of the markers. 
/// For instance, a cube of any size will be shrunk to 5 centimeters if ZEDArUcoDetectionManager.markerWidthMeters is set to 0.05.
/// </summary>
public class ScaleToMarkerSizeDesk : MonoBehaviour
{
    /// <summary>
    /// MeshFilter holding the mesh we'll use to set the scale reference. 
    /// Should be in this same object or a child of it. Make it a child if you want to add custom scaling to that object. 
    /// </summary>
    [Tooltip("MeshFilter holding the mesh we'll use to set the scale reference. " +
        "Should be in this same object or a child of it. Make it a child if you want to add custom scaling to that object.")]
    public MeshFilter filter;
    /// <summary>
    /// Bounds of the mesh. Shouldn't change after start. 
    /// </summary>
    private Bounds rendStartBounds;

    /// <summary>
    /// The scene's ZEDArUcoDetectionManager, for checking markerWidthMeters. Will be assigned automatically if left blank. 
    /// </summary>
    [Tooltip("The scene's ZEDArUcoDetectionManager, for checking markerWidthMeters. Will be assigned automatically if left blank. ")]
    public ZEDArUcoDetectionManager arucoManager;

    /// <summary>
    /// Size of markers as defined in ZEDArUcoDetectionManager. We cache it here to know if we should update it. 
    /// </summary>
    private float markerSize;

    public float multiplier = 1;
    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;



	// Use this for initialization
	void Start ()
    {
        if (!filter) filter = GetComponentInChildren<MeshFilter>();
        rendStartBounds = filter.mesh.bounds;

        if (!arucoManager) arucoManager = FindObjectOfType<ZEDArUcoDetectionManager>();
        if(!arucoManager)
        {
            Debug.LogError("No ZEDArUcoDetectionManager in the scene.");
        }
        markerSize = arucoManager.markerWidthMeters;

        SetScale();

    }

	// Update is called once per frame
	void Update ()
    {
        if(markerSize != arucoManager.markerWidthMeters)
        {
            markerSize = arucoManager.markerWidthMeters;
            SetScale();
        }
    }

    /// <summary>
    /// Changes the scale of this object so that the mesh object fits nicely in the markers. Assumes markerSize is updated. 
    /// </summary>
    private void SetScale()
    {
        float largest = (rendStartBounds.size.x > rendStartBounds.size.y) ? rendStartBounds.size.x : rendStartBounds.size.y;

        float scalemult = markerSize / largest * multiplier;

        transform.localScale = new Vector3(scalemult, scalemult, scalemult);
    }

    private void SetPosition()
    {
        Vector3 deskPosition = filter.GetComponent<Transform>().position;
        Vector3 cameraPosition = Camera.main.transform.position;
        filter.GetComponent<Transform>().position = filter.GetComponent<Transform>().position + (deskPosition - cameraPosition) * xOffset;
        Debug.Log("Mesh position: " + filter.GetComponent<Transform>().position);
        transform.position = transform.position + (transform.position - cameraPosition) * xOffset;
        Debug.Log("Desk position: " + transform.position);
    }
}
#endif