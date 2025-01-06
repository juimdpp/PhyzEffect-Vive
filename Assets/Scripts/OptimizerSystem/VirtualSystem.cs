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
        updateText();
    }

    private void updateText()
    {
        surfaceBouncinessCurrentValue.text = "Surface Bounciness: " + Surface.GetComponent<Collider>().material.bounciness.ToString();
        ballBouncinessCurrentValue.text = "Ball Bounciness: " + VirtualBall.GetComponent<Collider>().material.bounciness.ToString();
    }
    private void SurfaceIncrease()
    {
        Surface.GetComponent<Collider>().material.bounciness += 0.1f;
        updateText();
    }

    private void SurfaceDecrease()
    {
        Surface.GetComponent<Collider>().material.bounciness -= 0.1f;
        updateText();
    }

    private void BallIncrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness += 0.1f;
        updateText();
    }

    private void BallDecrease()
    {
        VirtualBall.GetComponent<Collider>().material.bounciness -= 0.1f;
        updateText();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (trackVirtual)
        {
            virtualCentroids.Add(Time.fixedTimeAsDouble, VirtualBall.GetComponent<Transform>().position);
        }
    }

    // Run all possible simulations using gridsearch. TODO: expand to other search methods
    public void RunAllSimulations(double duration, Vector3 pos)
    {
        StartCoroutine(SimulationCoroutine(duration, pos));   
    }

    public IEnumerator SimulationCoroutine(double duration, Vector3 pos)
    {
        LogAndDisplay($"Run each simulation for {duration} milliseconds");
        for (float ball = 0.9f; ball < 1; ball += 0.1f)
        {
            VirtualBall.GetComponent<Collider>().material.bounciness = ball;
            for (float surface = 0.5f; surface < 1; surface += 0.1f)
            {
                Surface.GetComponent<Collider>().material.bounciness = surface;

                SetStartingPosition(pos);
                ToStartPosition();
                DropBall(); // Start simulation

                // Wait until trackingDuration has elapsed
                yield return new WaitForSeconds((float)(duration/1000));
                StopTracking();
                SaveVirtualResults();
                LogAndDisplay($"Simulation finished for Ball Bounciness: {ball} and Surface Bounciness: {surface}");
            }
        }
        LogAndDisplay($"Finished all simulations");
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
        
        StartingPosition = VirtualBall.GetComponent<Transform>().position;
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
        string dirName = Path.GetDirectoryName(virtualCentroidFilePath);
        string extension = Path.GetExtension(virtualCentroidFilePath);
        // Initial file path
        string filePath = $"{dirName}/{baseFileName}_{ballBounciness}_{surfaceBounciness}_0{extension}";
        Debug.Log("start: " + filePath);
        // Check if the file exists, and increment the number if it does
        int counter = 0;
        while (File.Exists(filePath))
        {
            counter++;
            filePath = $"{dirName}/{baseFileName}_{ballBounciness}_{surfaceBounciness}_{counter}{extension}";
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
        virtualCentroids.Clear();
    }

    void LogAndDisplay(string str)
    {
        // statusText.text = str;
        Debug.Log(str);
    }
}
