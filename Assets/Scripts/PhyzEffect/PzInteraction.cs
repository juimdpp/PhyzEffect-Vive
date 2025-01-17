using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PzInteraction // FreeFall (bounceV, bounceR), Slide (frictionV, fricitonR, startForce)
{
    GameObject RealObj { get; set; }
    PzVirtualObject VirtObj { get; set; }
    Vector3 StartingPosition { get; set; }

    public delegate void Event();
    public event Event OnEndAllSimulations;

    
    // Start the automatic simulation process
    void StartAllSimulations();
    // Stop the automatic simulation process (
    void StopAllSimulations();
}
