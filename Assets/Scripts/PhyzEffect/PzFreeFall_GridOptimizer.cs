using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Makes GameObject to which it is attached to able to automatically simulate interaction (start - stop - update params)
public class PzFreeFall : MonoBehaviour, PzInteraction
{
    public GameObject RealObj { get; set; }
    public PzVirtualObject VirtObj { get; set; }
    public Vector3 StartingPosition { get; set; }
    public Vector3 EndPosition { get; set; } // Not used
    public SortedDictionary<double, Vector3> realTrajectory { get; set; }

    // Event
    public event PzInteraction.Event OnEndAllSimulations;

    private float gridStep = 0.1f; // TODO: make this controllable by PzOptimizer;
    private List<(float, float)> ParamsCombinations = new List<(float, float)>();
    private int currParamIdx = 0;
    private bool isSimulating = false;

    void Awake()
    {
        VirtObj = GetComponent<PzVirtualObject>();
        if (VirtObj != null)
        {
            VirtObj.OnRest += StopSingleSimulation; 
        }
        realTrajectory = new SortedDictionary<double, Vector3>();
    }
    
    public void StartOptimization()
    {
        isSimulating = true;
        Debug.Log("Start Auto Simulation (for freefall, start gravity, for friction, push motion)");

        // FreeFall has 2 parameters: VirtObj.bounciness, RealObj.bounciness. 
        // Create all possible combinations of the parameters using gridstep
        for (float r = 0.0f; r <= 1; r += gridStep)
        {
            for (float v = 0.0f; v <= 1; v += gridStep)
            {
                ParamsCombinations.Add((r, v));
            }
        }

        UpdateParams();
        StartSingleSimulation();
        // TODO: params
    }
    public void StopOptimization()
    {
        isSimulating = false;
        ResetEnv();
        VirtObj.GetComponent<Rigidbody>().Sleep();
    }

    // Set environment to default (gravity and position), then check if we should continue simulating or not.
    private void ResetEnv()
    {
        Debug.Log("ResetEnv");
        VirtObj.GetComponent<Rigidbody>().useGravity = false;
        VirtObj.GetComponent<Transform>().position = StartingPosition; // Set to initial position
    }

    // Start a single run of simulation
    private void StartSingleSimulation()
    {
        Debug.Log("StartSingleSimulation");
        isSimulating = true;
        VirtObj.GetComponent<Rigidbody>().useGravity = true;
    }
    // Called when a single simulation is finished (when object stands still)
    // Update parameter if we can, reset position to initial and start simulation. Else, notify that we've finished
    private void StopSingleSimulation()
    {
        Debug.Log("StopSingleSimulation");
        if (!isSimulating) return;
        ResetEnv();
        if (currParamIdx < ParamsCombinations.Capacity)
        {
            UpdateParams();
            StartSingleSimulation(); // Trigger a new simulation
        }
        else
        {
            OnEndAllSimulations?.Invoke();
        }
    }

    private void UpdateParams()
    {
        var tuple = ParamsCombinations[currParamIdx];
        Debug.Log("UpdateParams to " + tuple);
        VirtObj.GetComponent<Collider>().material.bounciness = tuple.Item1;
        RealObj.GetComponent<Collider>().material.bounciness = tuple.Item2;
        currParamIdx++;
        Debug.Log($"HELLO - {realTrajectory.Count}");
    }

    // void ReturnTrajectory();

}