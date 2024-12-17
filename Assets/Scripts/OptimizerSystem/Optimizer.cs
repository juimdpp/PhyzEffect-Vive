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
    public Vector3 startingPos;

    public Button GetRealBallTrajectoryBtn;
    public Button StartSimulateBtn;
    public Button EndSimulateBtn;
    public Button RunAllSimulationsBtn;
  
    public TMP_Text statusText;

    // Systems
    private VirtualSystem virtualSystem;
    private RealSystem realSystem;

    SortedDictionary<double, Vector3> realBallTrajectory = new SortedDictionary<double, Vector3>();
    // SortedDictionary<double, Vector3> virtBallTrajectory = new SortedDictionary<double, Vector3>();


    void Start()
    {
        virtualSystem = GetComponent<VirtualSystem>();
        realSystem = GetComponent<RealSystem>();

        GetRealBallTrajectoryBtn.onClick.AddListener(GetRealBallTrajectory);
        StartSimulateBtn.onClick.AddListener(StartSimulateTrajectory);
        EndSimulateBtn.onClick.AddListener(EndSimulateTrajectory);
        RunAllSimulationsBtn.onClick.AddListener(SimulateTrajectories);
    }

    // Simulate all possible trajectories using gridsearch (with 0.1 step)
    private void SimulateTrajectories()
    {
        // Assume this is called after getting RealTrajectory
        double duration = realBallTrajectory.Last().Key - realBallTrajectory.First().Key;
        virtualSystem.RunAllSimulations(duration, realBallTrajectory.First().Value);
    }


    void GetRealBallTrajectory()
    {
        Debug.Log("Getting RealBall's trajectory!" );
        
        realSystem.AutoRun();
        realBallTrajectory = realSystem.GetTransformedCentroids();
        realSystem.SaveRealResults();
        

        Debug.Log("Got RealBall's trajectory!" + realBallTrajectory.First().Key + " -- " + realBallTrajectory.First().Value);
    }

 

    // Simulate single trajectory
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


}
