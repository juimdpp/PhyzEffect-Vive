using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TempTrackMarker : MonoBehaviour
{
    public string RealFilePath;
    public string VirtualFilePath;
    public GameObject RefMarker;
    public GameObject VirtualBox;
    public Button RealTrack;
    public Button VirtualTrack;
    public Button Push;
    public Button Reset;
    public float startVelocity; // We assume X, Y velocity is 0

    private bool isRealTrack = false;
    private bool isVirtualTrack = false;
    private StreamWriter RealWriter;
    private StreamWriter VirtualWriter;
    private Vector3 initPosition;

    // Start is called before the first frame update
    void Start()
    {
        RealTrack.onClick.AddListener(onRealTrack);
        VirtualTrack.onClick.AddListener(onVirtualTrack);
        Push.onClick.AddListener(onPush);
        Reset.onClick.AddListener(onReset);

        

        // Open the file for writing
        RealWriter = new StreamWriter(RealFilePath, false); // Overwrite file if it exists

        VirtualWriter = new StreamWriter(VirtualFilePath, false); // Overwrite file if it exists
        Debug.Log($"Saving position data to: {VirtualFilePath}");
    }

    void onPush()
    {
        initPosition = VirtualBox.GetComponent<Transform>().position;
        VirtualBox.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, startVelocity));
    }
    void onReset()
    {
        VirtualBox.GetComponent<Transform>().position = initPosition;
    }
    void onRealTrack()
    {
        isRealTrack = !isRealTrack;
        // Ensure the file is closed when the application exits
        if (!isRealTrack && RealWriter != null)
        {
            RealWriter.Close();
        }
        if(isRealTrack && RealWriter != null)
        {
            RealWriter = new StreamWriter(RealFilePath, false);
        }
    }

    void onVirtualTrack()
    {
        isVirtualTrack = !isVirtualTrack;
        // Ensure the file is closed when the application exits
        if (!isVirtualTrack && VirtualWriter != null)
        {
            VirtualWriter.Close();
        }
        if (isVirtualTrack && VirtualWriter != null)
        {
            VirtualWriter = new StreamWriter(VirtualFilePath, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRealTrack)
        {
            // Get the position of the object in camera coordinates
            Vector3 camPosition = Camera.main.worldToCameraMatrix.MultiplyPoint(RefMarker.transform.position);

            // Record the position and timestamp
            float timestamp = Time.time;
            string line = $"{timestamp}, {camPosition.x}, {camPosition.y}, {camPosition.z}";
            Debug.Log(line);
            // Write the line to the file
            RealWriter.WriteLine(line);

            // Save and close file when the user presses "S"
            if (Input.GetKeyDown(KeyCode.S))
            {
                RealWriter.Close();
                Debug.Log("Position data saved and file closed.");
            }
        }
        if (isVirtualTrack)
        {
            // Get the position of the object in camera coordinates
            Vector3 camPosition = Camera.main.worldToCameraMatrix.MultiplyPoint(RefMarker.transform.position);

            // Record the position and timestamp
            float timestamp = Time.time;
            string line = $"{timestamp}, {camPosition.x}, {camPosition.y}, {camPosition.z}";
            Debug.Log(line);
            // Write the line to the file
            VirtualWriter.WriteLine(line);

            // Save and close file when the user presses "S"
            if (Input.GetKeyDown(KeyCode.S))
            {
                VirtualWriter.Close();
                Debug.Log("Position data saved and file closed.");
            }
        }
    }

    void OnApplicationQuit()
    {
        // Ensure the file is closed when the application exits
        if (VirtualWriter != null)
        {
            VirtualWriter.Close();
        }
        if (RealWriter != null)
        {
            RealWriter.Close();
        }
    }
}
