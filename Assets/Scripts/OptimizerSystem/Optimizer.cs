using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SystemManager : MonoBehaviour
{
    //// Algorithm related buttons
    //public Button PrepareTrajectoriesBtn;
    //public Button CompareTrajectoriesBtn;
    //public Button OptimizeBtn;
    public Vector3 startingPos;

    public Button GetRealBallTrajectoryBtn;
    public Button StartSimulateBtn;
    public Button EndSimulateBtn;
    public Button GetVirtBallTrajectoryBtn;
    
    public Button CompareTrajectoriesBtn;

    public TMP_Dropdown SearchTypeDropdown; // TODO
    public TMP_Dropdown SurfaceTypeDropdown;// TODO


    public TMP_Text statusText;

    // Filenames
    

    // Systems
    private VirtualSystem virtualSystem;
    private RealSystem realSystem;

    SortedDictionary<double, Vector3> realBallTrajectory = new SortedDictionary<double, Vector3>();
    SortedDictionary<double, Vector3> virtBallTrajectory = new SortedDictionary<double, Vector3>();

    private float yNew;

    void Start()
    {
        virtualSystem = GetComponent<VirtualSystem>();
        realSystem = GetComponent<RealSystem>();


        GetRealBallTrajectoryBtn.onClick.AddListener(GetRealBallTrajectory);
        GetVirtBallTrajectoryBtn.onClick.AddListener(GetVirtualBallTrajectory);
        StartSimulateBtn.onClick.AddListener(StartSimulateTrajectory);
        EndSimulateBtn.onClick.AddListener(EndSimulateTrajectory);


        //PrepareTrajectoriesBtn.onClick.AddListener(algorithmSystem.PrepareTrajectories);
        CompareTrajectoriesBtn.onClick.AddListener(CompareTrajectories);
        //OptimizeBtn.onClick.AddListener(algorithmSystem.Optimize);

    }


    // TODO: erase this!
    void LoadCentroidsFromFile()
    {
        if (File.Exists(virtualSystem.transformedCentroidFilePath))
        {
            string[] lines = File.ReadAllLines(virtualSystem.transformedCentroidFilePath);
            bool start = true;
            foreach (string line in lines)
            {
                if (start)
                {
                    start = false;
                    continue;
                }
                string[] split = line.Split(',');
                if (split.Length == 4)
                {
                    // Parse the x, y, z centroid coordinates from the file
                    float x = float.Parse(split[1]);
                    float y = float.Parse(split[2]);
                    float z = float.Parse(split[3]);
                    realBallTrajectory.Add(double.Parse(split[0]), new Vector3(x, y, z));  // Add to the list of centroids
                }
            }
            virtualSystem.SetStartingPosition(realBallTrajectory.First().Value);
            Debug.Log("Loaded real centroids and setStartingPosition " + realBallTrajectory.Count);
        }
        else
        {
            Debug.LogError("Centroid file not found at: " + virtualSystem.transformedCentroidFilePath);
        }
    }
    // centroids.txt -> transformedCentroids
    void GetRealBallTrajectory()
    {
        Debug.Log("Getting RealBall's trajectory!" );
        // TODO: uncomment this. Currently using same real-world trajectory for debugging
        /*realSystem.AutoRun();
        realBallTrajectory = realSystem.GetTransformedCentroids();
        realSystem.SaveRealResults();
        */
        
        LoadCentroidsFromFile();
        // END OF TODO: erase

        Debug.Log("Got RealBall's trajectory!" + realBallTrajectory.First().Key + " -- " + realBallTrajectory.First().Value);
    }

 

    void StartSimulateTrajectory()
    {
        // Get StartingPosition of RealBall
        virtualSystem.SetStartingPosition(realBallTrajectory.First().Value);
        // Simulate virtual ball
        virtualSystem.ToStartPosition();
        virtualSystem.DropBall();
        Debug.Log("Simulating VirtBall's trajectory");
    }

    void EndSimulateTrajectory()
    {
        virtualSystem.StopTracking();
        virtualSystem.SaveVirtualResults();
    }

    void GetVirtualBallTrajectory()
    {
        virtBallTrajectory = virtualSystem.GetSimulatedVirtualBallTrajectory();
        Debug.Log("Got VirtBall's trajectory!");
    }

    void CompareTrajectories()
    {
        Debug.Log("Assume that even RealBall moves after first frame");

        
        Debug.Log("Virtual Ball Trajectory");
        for (int i=0; i<5; i++)
        {
            Debug.Log(virtBallTrajectory.ElementAt(i));
        }
        Debug.Log("Real Ball Trajectory");
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(realBallTrajectory.ElementAt(i));
        }
    }

    void Optimize()
    {

    }

    // Use case
    /*
     * AutoRun -> pass transformedCentroids to virtualSystem
     * */


}
