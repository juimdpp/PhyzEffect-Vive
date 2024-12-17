using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualSystem : MonoBehaviour
{
    
    public string transformedCentroidFilePath = "Assets/MyAssets/transformedCentroids.txt";
    public string virtualCentroidFilePath = "GitIgnoredFiles/virtualCentroids.txt";
    public GameObject VirtualBall;
    public GameObject Surface;
    public TMP_Text statusText;

    // Virtual-object related buttons
    public Button SurfaceIncreaseBtn;
    public Button SurfaceDecreaseBtn;
    public TMP_Text surfaceBouncinessCurrentValue;
    public Button BallIncreaseBtn;
    public Button BallDecreaseBtn;
    public TMP_Text ballBouncinessCurrentValue;
    
    public TMP_Text massAndSize;


    private SortedDictionary<double, Vector3> virtualCentroids = new SortedDictionary<double, Vector3>();  // Virtual coordinates
    private bool trackVirtual = false;
    private Vector3 StartingPosition = new Vector3(0, 0, 0);
    

    // Start is called before the first frame update
    void Start()
    {
        // Set mass and size
        massAndSize.text = $"{VirtualBall.GetComponent<Rigidbody>().mass}g || {VirtualBall.GetComponent<Transform>().localScale}";
        // Surface Button click listeners
        SurfaceIncreaseBtn.onClick.AddListener(SurfaceIncrease);
        SurfaceDecreaseBtn.onClick.AddListener(SurfaceDecrease);

        // Button click listeners
        BallIncreaseBtn.onClick.AddListener(BallIncrease);
        BallDecreaseBtn.onClick.AddListener(BallDecrease);

        // Default values
        surfaceBouncinessCurrentValue.text = "surface bounciness";
        ballBouncinessCurrentValue.text = "ball bounciness";
    }

    private void SurfaceIncrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness += 0.1f;
        surfaceBouncinessCurrentValue.text = Surface.GetComponent<Collider>().material.bounciness.ToString();
    }

    private void SurfaceDecrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness -= 0.1f;
        surfaceBouncinessCurrentValue.text = Surface.GetComponent<Collider>().material.bounciness.ToString();
    }

    private void BallIncrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness += 0.1f;
        ballBouncinessCurrentValue.text = VirtualBall.GetComponent<Collider>().material.bounciness.ToString();
    }

    private void BallDecrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness -= 0.1f;
        ballBouncinessCurrentValue.text = VirtualBall.GetComponent<Collider>().material.bounciness.ToString();
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
        Debug.Log("Starting Position: " + StartingPosition);
    }

    public void ToStartPosition()
    {
        VirtualBall.GetComponent<Transform>().position =  new Vector3(StartingPosition.x, StartingPosition.y, StartingPosition.z);
        LogAndDisplay("Changed VirtualBall position to " + VirtualBall.GetComponent<Transform>().position);
    }
    public void DropBall() // And also start tracking
    {
        trackVirtual = true;
        VirtualBall.GetComponent<Rigidbody>().useGravity = true;
        LogAndDisplay("Drop VirtualBall and start tracking");
        Debug.Log("Bounciness: " + VirtualBall.GetComponent<Collider>().material.bounciness);
    }
    public void StopTracking() {
        VirtualBall.GetComponent<Rigidbody>().useGravity = false;
        trackVirtual = false;
        VirtualBall.GetComponent<Transform>().position = StartingPosition;
        LogAndDisplay("Stop tracking VirtualBall");
    }
    public void SaveVirtualResults() {
        float surfaceBounciness = Surface.GetComponent<Collider>().material.bounciness;
        float ballBounciness = VirtualBall.GetComponent<Collider>().material.bounciness;
        string baseFileName = Path.GetFileNameWithoutExtension(virtualCentroidFilePath);
        string extension = Path.GetExtension(virtualCentroidFilePath);
        // Initial file path
        string filePath = $"{baseFileName}_{ballBounciness}_{surfaceBounciness}_0{extension}";
        Debug.Log("start: " + filePath);
        // Check if the file exists, and increment the number if it does
        int counter = 0;
        while (File.Exists(filePath))
        {
            counter++;
            filePath = $"{baseFileName}_{ballBounciness}_{surfaceBounciness}_{counter}{extension}";
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
