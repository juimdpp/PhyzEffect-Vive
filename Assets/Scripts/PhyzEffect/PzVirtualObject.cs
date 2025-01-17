using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes the GameObject to which this is attached to send notifications when still
[RequireComponent(typeof(Rigidbody))]
public class PzVirtualObject : MonoBehaviour
{
    public delegate void Event();
    public event Event OnRest;
    public event Event OnInfMovement; // This works for bounciness (FreeFall). Haven't checked if it's applicable to other interactions as well

    private bool notified = false;
    private float timeSinceLastCollision = 0.0f;
    private float infMvtThreshold = 5f;

    void Update()
    {
        if (GetComponent<Rigidbody>().IsSleeping())
        {
            if (!notified)
            {
                Debug.Log("Notify still");
                OnRest?.Invoke();
                notified = true;
            }
        }
        else
        {
            if (notified)
            {
                Debug.Log("Started to move");
            }
            notified = false;

            timeSinceLastCollision += Time.deltaTime;
            if(timeSinceLastCollision > infMvtThreshold)
            {
                Debug.Log("InfMovement");
                OnInfMovement?.Invoke();
                timeSinceLastCollision = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        timeSinceLastCollision = 0f;
    }

}
