using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualSystem : MonoBehaviour
{
    
    public string transformedCentroidFilePath = "Assets/MyAssets/transformedCentroids.txt";
    public string virtualCentroidFilePath = "Assets/MyAssets/virtualCentroids.txt";
    public GameObject VirtualBall;
    public GameObject Desk;
    public TMP_Text statusText;

    // Virtual-object related buttons
    public Button ToStartPositionBtn;
    public Button DropBallBtn;
    public Button FindOptimalStartingPositionBtn;
    //public Button StopTrackingBtn;
    //public Button SaveVirtualResultsBtn;

    private SortedDictionary<double, Vector3> virtualCentroids = new SortedDictionary<double, Vector3>();  // Virtual coordinates
    private bool trackVirtual = false;
    private Vector3 StartingPosition = new Vector3(0, 0, 0);

    private Vector3 initPosition;

    // Start is called before the first frame update
    void Start()
    {
        initPosition = VirtualBall.GetComponent<Transform>().position;
        // Button click listeners
        ToStartPositionBtn.onClick.AddListener(ToStartPosition);
        DropBallBtn.onClick.AddListener(DropBall);
        
        //StopTrackingBtn.onClick.AddListener(StopTracking);
        //SaveVirtualResultsBtn.onClick.AddListener(SaveVirtualResults);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackVirtual)
        {
            virtualCentroids.Add(Time.fixedTimeAsDouble, VirtualBall.GetComponent<Transform>().position);
        }
    }

    public SortedDictionary<double, Vector3> GetSimulatedVirtualBallTrajectory()
    {
        if (trackVirtual)
        {
            LogAndDisplay("Wait until virtual ball is idle!");
            return null;
        }
        else
        {
            return virtualCentroids;
        }
    }


    public void SetStartingPosition(Vector3 pos)
    {
        StartingPosition = pos;
        initPosition = VirtualBall.GetComponent<Transform>().position;
        Debug.Log("Starting Position: " + StartingPosition);
    }

    public void ToStartPosition()
    {
        VirtualBall.GetComponent<Transform>().position =  new Vector3(StartingPosition.x, StartingPosition.y, StartingPosition.z);
        LogAndDisplay("Changed VirtualBall position to " + VirtualBall.GetComponent<Transform>().position + " from " + initPosition);
    }
    public void DropBall() // And also start tracking
    {
        VirtualBall.GetComponent<Rigidbody>().useGravity = true;
        trackVirtual = true;
        LogAndDisplay("Drop VirtualBall and start tracking");
        Debug.Log("Bounciness: " + VirtualBall.GetComponent<SphereCollider>().material.bounciness);
    }
    public void StopTracking() {
        trackVirtual = false;
        VirtualBall.GetComponent<Rigidbody>().useGravity = false;
        VirtualBall.GetComponent<Transform>().position = initPosition;
        LogAndDisplay("Stop tracking VirtualBall");
    }
    public void SaveVirtualResults() {
        float bounciness = Desk.GetComponent<MeshCollider>().material.bounciness;
        string baseFileName = Path.GetFileNameWithoutExtension(virtualCentroidFilePath);
        string extension = Path.GetExtension(virtualCentroidFilePath);
        // Initial file path
        string filePath = $"{baseFileName}_{StartingPosition.y}_{bounciness}_0{extension}";
        Debug.Log("start: " + filePath);
        // Check if the file exists, and increment the number if it does
        int counter = 0;
        while (File.Exists(filePath))
        {
            counter++;
            filePath = $"{baseFileName}_{StartingPosition.y}_{bounciness}_{counter}{extension}";
            Debug.Log(counter + ": " + filePath);
        }
        
        using (StreamWriter writer = new StreamWriter(filePath, false))  // true to append
        {
            writer.WriteLine("timestamp,x,y,z");  // Writing headers
            foreach (KeyValuePair<double, Vector3> pair in virtualCentroids)
            {
                Vector3 centroid = pair.Value;
                // Write each transformed centroid in x,y,z format
                writer.WriteLine($"{pair.Key},{centroid.x},{centroid.y},{centroid.z}");
            }
        }
        LogAndDisplay("Virtual centroids saved to " + filePath);
    }

    void LogAndDisplay(string str)
    {
        statusText.text = str;
        Debug.Log(str);
    }
}
