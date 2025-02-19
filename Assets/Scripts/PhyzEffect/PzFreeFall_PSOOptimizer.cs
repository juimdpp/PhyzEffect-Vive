using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PzPSOOptimizer : MonoBehaviour, PzInteraction
{
    public GameObject RealObj { get; set; }
    public PzVirtualObject VirtObj { get; set; }
    public Vector3 StartingPosition { get; set; }
    public Vector3 EndPosition { get; set; } // Not used
    public SortedDictionary<double, Vector3> realTrajectory { get; set; }
    public SortedDictionary<double, Vector3> currSimTrajectory { get; set; }

    // Event
    public event PzInteraction.Event OnEndAllSimulations;

    private bool isSimulating = false;

    // Optimizations
    private List<Particle> swarm = new List<Particle>();
    private int swarmSize = 10;
    private (float, float) globalBestPosition;
    private float globalBestError = 1.0f;
    private int maxIteration = 5;
    private int currIter = 0;
    private int currParticleIdx = 0;
    

    void Awake()
    {
        VirtObj = GetComponent<PzVirtualObject>();
        if (VirtObj != null)
        {
            VirtObj.OnRest += StopSingleSimulation;
        }
        realTrajectory = new SortedDictionary<double, Vector3>();
        currSimTrajectory = new SortedDictionary<double, Vector3>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSimulating)
        {
            currSimTrajectory[Time.fixedTimeAsDouble] = VirtObj.transform.position;
        }
    }

    public void StartOptimization()
    {
        SimulationUtils.LoadFile("simpleFreeFallTrajectory.txt", realTrajectory);
        Debug.Log($"Load real trajectory in PSO Optimizer: {realTrajectory.Count}");

        Debug.Log("Start PSO Optimization for freefall");

        PSO_Initialize();
        PSO();
    }
    public void StopOptimization()
    {
        isSimulating = false;
        ResetEnv();
        VirtObj.GetComponent<Rigidbody>().Sleep();
    }

    private void PSO_Initialize()
    {
        // Initialize
        for(int i=0; i<swarmSize; i++)
        {
            swarm.Add(new Particle(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }
        globalBestPosition = swarm[0].bestPosition;

        // PSO
        currParticleIdx = 0;
        currIter = 0;
    }

    private void PSO()
    {
        ResetEnv();
        // Update velocity
        swarm[currParticleIdx].UpdateVelocity(globalBestPosition);
        // Update position
        swarm[currParticleIdx].UpdatePosition();
        // Set parameters for simulation
        UpdateParams();
        // Simulate particle 
        StartSingleSimulation();
    }


    // Set environment to default (gravity and position) and empty tracked simulated trajectory
    private void ResetEnv()
    {
        VirtObj.GetComponent<Rigidbody>().useGravity = false;
        VirtObj.GetComponent<Transform>().position = realTrajectory.First().Value; // Set to initial position
        currSimTrajectory.Clear();
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
        
        if (currIter < maxIteration)
        {
            if(currParticleIdx < swarm.Count)
            {
                // Calculate error
                float err = CalculateError();
                if(err < swarm[currParticleIdx].bestError)
                {
                    swarm[currParticleIdx].bestPosition = swarm[currParticleIdx].position;
                    swarm[currParticleIdx].bestError = err;
                    if(err < globalBestError)
                    {
                        globalBestError = err;
                        globalBestPosition = swarm[currParticleIdx].bestPosition;
                        Debug.Log($"BEST POSITION: {currIter} iteration, {currParticleIdx} index ({globalBestPosition}) error: {err}");
                    }
                }
                currParticleIdx++;
                if(currParticleIdx >= swarm.Count)
                {
                    currIter++;
                    currParticleIdx = 0;
                }
                PSO(); // Trigger a new simulation
            }            
        }
        else
        {
            StopOptimization();
        }
    }

    private float CalculateError()
    {
        if(currSimTrajectory.Count == 0 || realTrajectory.Count == 0)
        {
            Debug.LogWarning("One or both trajectories are empty. Please check.");
            return float.MaxValue;
        }

        // Normalize timestamps
        Dictionary<double, Vector3> realNormTraj = NormalizeTimestamps(realTrajectory);
        Dictionary<double, Vector3> simNormTraj = NormalizeTimestamps(currSimTrajectory);

        float error = 0.0f;

        // Compare for each timestamp in realTrajectory
        foreach(var (realTime, realPosition) in realNormTraj)
        {
            Vector3 simPosition = InterpolateSimTimestamp(realTime, simNormTraj);

            error += (realPosition - simPosition).sqrMagnitude;
        }

        float mse = error/realTrajectory.Count;
        Debug.Log($"{currIter} iteration, {currParticleIdx} index ({swarm[currParticleIdx]}) error: {mse}");
        SimulationUtils.SaveToFile($"{currIter},{currParticleIdx}_simTrajectory_{swarm[currParticleIdx].bestPosition}_{mse}.txt", "timestamp,x,y,z", currSimTrajectory);
        SimulationUtils.SaveToFile("realTrajectory.txt", "timestamp,x,y,z", realTrajectory);
        return mse;
    }

    private Vector3 InterpolateSimTimestamp(double realTime, Dictionary<double, Vector3> simNormTraj)
    {
        double[] timestamps = simNormTraj.Keys.ToArray();
        for (int i=0; i<timestamps.Length-1; i++)
        {
            if(timestamps[i] <= realTime && realTime <= timestamps[i+1])
            {
                float factor = (float)((realTime - timestamps[i]) / (timestamps[i+1] - timestamps[i]));
                return Vector3.Lerp(simNormTraj[timestamps[i]], simNormTraj[timestamps[i + 1]], factor);
            }
        }
        return simNormTraj.ContainsKey(timestamps[0]) ? simNormTraj[timestamps[0]] :
            simNormTraj.ContainsKey(timestamps[^1]) ? simNormTraj[timestamps[^1]] :
            Vector3.zero;
    }

    private Dictionary<double, Vector3> NormalizeTimestamps(SortedDictionary<double, Vector3> trajectory)
    {
        Dictionary<double, Vector3> normalized = new Dictionary<double, Vector3>();
        double start = trajectory.First().Key;
        foreach(var (time, vec) in trajectory)
        {
            normalized.Add(time - start, vec);
        }
        return normalized;
    }

    private void UpdateParams()
    {
        VirtObj.GetComponent<Collider>().material.bounciness = swarm[currParticleIdx].position.Item1;
        RealObj.GetComponent<Collider>().material.bounciness = swarm[currParticleIdx].position.Item2;
    }
}

class Particle
{
    public (float, float) position;
    public (float, float) bestPosition;
    public (float, float) velocity;
    public float bestError = 1.0f;

    // Parameters
    private float cognitiveCoefficient = 0.5f;
    private float socialCoefficient = 0.5f;
    private float inertiaWeight = 0.5f;

    public Particle(float a, float b)
    {
        position = (a, b);
        bestPosition = position;
        velocity = ((UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)));
    }

    public void UpdateVelocity((float, float) global)
    {
        float rP = cognitiveCoefficient * UnityEngine.Random.Range(0f, 1f);
        float rG = socialCoefficient * UnityEngine.Random.Range(0f, 1f);

        velocity.Item1 = inertiaWeight * velocity.Item1 + rP * (bestPosition.Item1 - position.Item1) + rG * (global.Item1 - position.Item1);
        velocity.Item2 = inertiaWeight * velocity.Item2 + rP * (bestPosition.Item2 - position.Item2) + rG * (global.Item2 - position.Item2);
    }

    public void UpdatePosition()
    {
        position.Item1 += velocity.Item1;
        position.Item2 += velocity.Item2;
    }
}