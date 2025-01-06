using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TempTrackMarker : MonoBehaviour
{
    public string filePath;
    public GameObject RefMarker;
    public Button Track;

    private bool isTrack = false;
    private StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        Track.onClick.AddListener(onTrack);

        // Open the file for writing
        string fullPath = Path.Combine(Application.dataPath, filePath);
        writer = new StreamWriter(filePath, false); // Overwrite file if it exists
        Debug.Log($"Saving position data to: {filePath}");
    }

    void onTrack()
    {
        isTrack = !isTrack;
        // Ensure the file is closed when the application exits
        if (!isTrack && writer != null)
        {
            writer.Close();
        }
        if(isTrack && writer != null)
        {
            writer = new StreamWriter(filePath, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrack)
        {
            // Get the position of the object in camera coordinates
            Vector3 camPosition = Camera.main.worldToCameraMatrix.MultiplyPoint(RefMarker.transform.position);

            // Record the position and timestamp
            float timestamp = Time.time;
            string line = $"{timestamp}, {camPosition.x}, {camPosition.y}, {camPosition.z}";
            Debug.Log(line);
            // Write the line to the file
            writer.WriteLine(line);

            // Save and close file when the user presses "S"
            if (Input.GetKeyDown(KeyCode.S))
            {
                writer.Close();
                Debug.Log("Position data saved and file closed.");
            }
        }
    }

    void OnApplicationQuit()
    {
        // Ensure the file is closed when the application exits
        if (writer != null)
        {
            writer.Close();
        }
    }
}
