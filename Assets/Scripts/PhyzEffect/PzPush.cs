using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PzPush : MonoBehaviour, PzInteraction
{
    public GameObject RealObj { get; set; }
    public PzVirtualObject VirtObj { get; set; }
    public Vector3 StartingPosition { get; set; }
    public Vector3 EndPosition { get; set; }
    public SortedDictionary<double, Vector3> realTrajectory { get; set; }
    public event PzInteraction.Event OnEndAllSimulations;

    private float gridStep = 0.1f; // TODO: make this controllable by PzOptimizer;
    private List<(float, float, float)> ParamsCombinations = new List<(float, float, float)>(); // VirtObj.dynamicFriction, RealObj.dynamicFriction
    private int currParamIdx = 0;
    private bool isSimulating = false;
    private Vector3 direction;
    private float currForce;

    void Awake()
    {
        VirtObj = GetComponent<PzVirtualObject>();
        if (VirtObj != null)
        {
            VirtObj.OnRest += StopSingleSimulation;
        }
    }
    public void StartOptimization() {
        Debug.Log("HYUNSOO - PZPUSH: StartAllSimulations");
        isSimulating = true;
        direction = (EndPosition - StartingPosition).normalized;

        // FreeFall has 2 parameters: VirtObj.bounciness, RealObj.bounciness. 
        // Create all possible combinations of the parameters using gridstep
        for (float r = 0.1f; r <= 1; r += gridStep)
        {
            for (float v = 0.1f; v <= 1; v += gridStep)
            {
                for(float f = 1; f <= 2; f += gridStep)
                {
                    ParamsCombinations.Add((r, v, f));
                }
            }
        }

        UpdateParams();
        StartSingleSimulation();
        // TODO: params
    }
    public void StopOptimization() {
        Debug.Log("HYUNSOO - PZPUSH: StopAllSimulations");
        isSimulating = false;
        ResetEnv();
        VirtObj.GetComponent<Rigidbody>().Sleep();
    }

    // Set environment to default (gravity and position), then check if we should continue simulating or not.
    private void ResetEnv()
    {
        VirtObj.GetComponent<Transform>().position = StartingPosition; // Set to initial position
        Debug.Log($"HYUNSOO - PZPUSH: ResetEnv to {StartingPosition}");
    }

    private void UpdateParams()
    {
        var tuple = ParamsCombinations[currParamIdx];
        Debug.Log("HYUNSOO - PZPUSH: UpdateParams to " + tuple);
        VirtObj.GetComponent<Collider>().material.staticFriction = tuple.Item1;
        VirtObj.GetComponent<Collider>().material.dynamicFriction = tuple.Item1;
        RealObj.GetComponent<Collider>().material.staticFriction = tuple.Item2;
        RealObj.GetComponent<Collider>().material.dynamicFriction = tuple.Item2;
        currForce = tuple.Item3;
        currParamIdx++;
    }

    // Start a single run of simulation
    private void StartSingleSimulation()
    {
        Debug.Log("HYUNSOO - PZPUSH: StartSingleSimulation");
        isSimulating = true;
        // TODO:
        
        VirtObj.GetComponent<Rigidbody>().AddForce(direction * currForce);
    }

    // Called when a single simulation is finished (when object stands still)
    // Update parameter if we can, reset position to initial and start simulation. Else, notify that we've finished
    private void StopSingleSimulation()
    {
        Debug.Log("HYUNSOO - PZPUSH: StopSingleSimulation");
        if (!isSimulating) return;
        ResetEnv();
        if (currParamIdx < ParamsCombinations.Count)
        {
            UpdateParams();
            StartSingleSimulation(); // Trigger a new simulation
        }
        else
        {
            OnEndAllSimulations?.Invoke();
        }
    }


}
