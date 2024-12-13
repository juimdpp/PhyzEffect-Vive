using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RealSystem : MonoBehaviour
{
    public string centroidFilePath = "Assets/MyAssets/centroids.txt";
    public string transformedCentroidFilePath = "Assets/MyAssets/transformedCentroids.txt";
    public TMP_Text statusText;  // UI Text to show the current status or message
    public GameObject Ref; // Typically camera

    //// Real-object related buttons
    //public Button AutoRunBtn;
    //public Button NextFrameBtn;
    //public Button PrevFrameBtn;
    //public Button SaveRealResultsBtn;

    private SortedDictionary<double, Vector3> centroids = new SortedDictionary<double, Vector3>();  // List to hold centroid data
    private SortedDictionary<double, Vector3> transformedCentroids = new SortedDictionary<double, Vector3>();  // Transformed world coordinates
    private int currentFrame = 0;  // To simulate the frame-by-frame transformation

    // Start is called before the first frame update
    void Start()
    {
        LoadCentroidsFromFile();  // Load centroids from the text file

        //AutoRunBtn.onClick.AddListener(AutoRun);
        //NextFrameBtn.onClick.AddListener(NextFrame);
        //PrevFrameBtn.onClick.AddListener(PrevFrame);
        //SaveRealResultsBtn.onClick.AddListener(SaveRealResults);
    }

    // Load the centroid data from a text file
    void LoadCentroidsFromFile()
    {
        if (File.Exists(centroidFilePath))
        {
            string[] lines = File.ReadAllLines(centroidFilePath);
            foreach (string line in lines)
            {
                string[] split = line.Split(',');
                if (split.Length == 4)
                {
                    // Parse the x, y, z centroid coordinates from the file
                    float x = float.Parse(split[1]);
                    float y = float.Parse(split[2]);
                    float z = float.Parse(split[3]);
                    centroids.Add(double.Parse(split[0]), new Vector3(x, y, z));  // Add to the list of centroids
                }
            }
            Debug.Log("Loaded centroids " + centroids.Count);
        }
        else
        {
            Debug.LogError("Centroid file not found at: " + centroidFilePath);
        }
    }

    // TODO: change return type to SortedDictionary<double, Vector3>
    public SortedDictionary<double, Vector3> GetTransformedCentroids()
    {
        return transformedCentroids;
    }
    public void AutoRun()
    {
        for (currentFrame = 0; currentFrame < centroids.Count; currentFrame++)
        {
            updateFrame();
        }
        currentFrame = 0;
    }
    public void NextFrame() {
        currentFrame++;
        updateFrame();
    }
    public void PrevFrame() {
        currentFrame--;
        updateFrame();
    }
    public void SaveRealResults() {
        // Append the transformed centroids data to the file
        using (StreamWriter writer = new StreamWriter(transformedCentroidFilePath, false))  // true to append
        {
            writer.WriteLine("timestamp,x,y,z");  // Writing headers
            foreach (KeyValuePair<double, Vector3> pair in transformedCentroids)
            {
                Vector3 centroid = pair.Value;
                // Write each transformed centroid in x,y,z format
                writer.WriteLine($"{pair.Key},{centroid.x},{centroid.y},{centroid.z}");
            }
        }
        LogAndDisplay("Transformed centroids saved to " + transformedCentroidFilePath);
    }

    void LogAndDisplay(string str)
    {
        statusText.text = str;
        Debug.Log(str);
    }

    void updateFrame()
    {
        statusText.text = currentFrame.ToString();
        if (currentFrame < centroids.Count && currentFrame >= 0)
        {
            double key = centroids.ElementAt(currentFrame).Key;
            Vector3 localCentroid = centroids[key];  // Get the centroid for the current frame

            // Convert to world space using the reference object
            Vector3 worldCentroid = Ref.transform.TransformPoint(localCentroid);
            // worldCentroid *= -1;
            transformedCentroids.Add(key, worldCentroid);  // Save the transformed world coordinates

            // Update the position of the game object based on the transformed centroid
            transform.position = worldCentroid;
        }
        else
        {
            // Reset when all centroids have been processed
            currentFrame = 0;
            LogAndDisplay("Transformation Completed!");
        }
    }


}
