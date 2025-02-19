using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Implement the below functions:
 *  - StartAllSimulations:
 *  - StopAllSimulations:
 *  - void Awake()
 */
public interface PzInteraction // FreeFall (bounceV, bounceR), Slide (frictionV, fricitonR, startForce)
{
    GameObject RealObj { get; set; }
    PzVirtualObject VirtObj { get; set; }
    Vector3 StartingPosition { get; set; }
    Vector3 EndPosition { get; set; }
    SortedDictionary<double, Vector3> realTrajectory { get; set; }

    public delegate void Event();
    public event Event OnEndAllSimulations;

    
    // Start the automatic simulation process
    void StartOptimization();
    // Stop the automatic simulation process (
    void StopOptimization();
}
