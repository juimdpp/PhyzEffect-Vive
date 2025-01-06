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

    public Button StartSimulateBtn;
    public Button EndSimulateBtn;
  
    public TMP_Text statusText;

    private bool start = true;

    // Systems
    private VirtualSystem virtualSystem;
    // private RealSystem realSystem;

    // SortedDictionary<double, Vector3> realBallTrajectory = new SortedDictionary<double, Vector3>();
    // SortedDictionary<double, Vector3> virtBallTrajectory = new SortedDictionary<double, Vector3>();


    void Start()
    {
        virtualSystem = GetComponent<VirtualSystem>();
        // realSystem = GetComponent<RealSystem>();

        StartSimulateBtn.onClick.AddListener(StartSimulateTrajectory);
        EndSimulateBtn.onClick.AddListener(EndSimulateTrajectory);
    }


 

    // Simulate single trajectory
    void StartSimulateTrajectory()
    {
        if (start)
        {
            // Get StartingPosition of RealBall
            virtualSystem.SetStartingPosition(new Vector3(0.0f, 0.0f, 0.0f));
            start = false;
        }
        
        // Simulate virtual ball
        virtualSystem.ToStartPosition();
        virtualSystem.DropBall();
        Debug.Log("Simulating VirtBall's trajectory");
    }

    void EndSimulateTrajectory()
    {
        virtualSystem.StopTracking();
        // virtualSystem.SaveVirtualResults();
    }


}
