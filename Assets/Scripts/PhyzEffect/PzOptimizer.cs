using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SearchMethod
{
    GridSearch
};

public enum InteractionType
{
    Pz_Bounciness_FreeFall,
    Pz_Bounciness_Push,
    Pz_Bounciness_Throw
}

// Makes the GameObject to which this is attached to send notifications when still
public class PzVirtualObject: MonoBehaviour
{
    public delegate void Event();

    public event Event OnRest;

    void Update()
    {
        if (gameObject.GetComponent<Rigidbody>().IsSleeping())
        {
            Debug.Log("Object is still");
            OnRest?.Invoke();
        }
        else
        {
            Debug.Log("Objet is moving");
        }
    }
}

public static class SimulationUtils
{
    public static void SaveToFile(string filePath, string header, SortedDictionary<double, Vector3> data)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine(header);
            foreach (var entry in data)
            {
                writer.WriteLine($"{entry.Key},{entry.Value.x},{entry.Value.y},{entry.Value.z}");
            }
        }
        Debug.Log($"Data saved to {filePath}");
    }
}
public interface PzInteraction // FreeFall (bounceV, bounceR), Slide (frictionV, fricitonR, startForce)
{
    GameObject RealObj { get; set; }
    PzVirtualObject VirtObj { get; set; }
    
    public void ResetParams();
    public void StartSimulation();
    public void StopSimulation();
}

// Makes GameObject to which it is attached to able to automatically simulate interaction (start - stop - update params)
public class PzFreeFall: MonoBehaviour, PzInteraction{
    public GameObject RealObj { get; set; }
    public PzVirtualObject VirtObj { get; set; }
    
    void Awake()
    {
        VirtObj = GetComponent<PzVirtualObject>();
    }
    void Start()
    {
        VirtObj.OnRest += StopSimulation;
    }

    public void ResetParams()
    {
        CanRun();
        Debug.Log("Reset parameters");
    }
    public void StartSimulation()
    {
        CanRun();
        Debug.Log("Start Simulation (for freefall, start gravity, for friction, push motion)");
        VirtObj.GetComponent<Rigidbody>().useGravity = true;
    }
    public void StopSimulation()
    {
        CanRun();
        Debug.Log("Stop Simulation (for freefall, stop gravity, for friction, stop motion)");
        VirtObj.GetComponent<Rigidbody>().useGravity = false;
    }

  
    private bool CanRun()
    {
        return true;
    }
}

public class PzOptimizer : MonoBehaviour
{
    // UI
    public Button StartAutoSimBtn;
    public Button StopAutoSimBtn;
    public TMP_Dropdown ChangeInteractionDpdwn; 
    public TMP_Dropdown SearchMethodDpdwn;
    // GameObjects
    public GameObject VirtualBall;
    public GameObject VirtualBox; // Access VirtualObject via CurrentInteraction.VirtObj;
    private PzVirtualObject VirtualObject;
    public GameObject RealObject;
    // User inputs (can be changed to private)
    public string SimulatedFilePath;

    // Interactions
    private PzInteraction CurrentInteraction;
    private PzFreeFall Bounciness_Freefall;

    // Other private stuff
    private Vector3 StartingPosition;
    private SortedDictionary<double, Vector3> trajectory = new SortedDictionary<double, Vector3>();
    private bool isSimulating;

    // Start is called before the first frame update
    void Start()
    {

        // Make the VirtualBall and VirtualBox PzInteractables
        VirtualBall.AddComponent<PzVirtualObject>();
        VirtualBox.AddComponent<PzVirtualObject>();

        // Bounciness_Freefall.Initialize(VirtualBall); // Use VirtualBall as default. Can change via dropdown

        // UI handlers
        StartAutoSimBtn.onClick.AddListener(StartAutoSimHandler);
        StopAutoSimBtn.onClick.AddListener(StopAutoSimHandler);
        ChangeInteractionDpdwn.onValueChanged.AddListener(ChangeInteractionHandler);
        SearchMethodDpdwn.onValueChanged.AddListener(SearchMethodHandler);

        // Triggered after scanned scene is positioned and scaled
        PositionAndScaleObject.OnPositionUpdated += UpdateStartingPosition;

        // Initialize to FreeFall by default
        ChangeInteractionHandler(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSimulating)
        {
            trajectory[Time.fixedTimeAsDouble] = CurrentInteraction.VirtObj.transform.position;
        }
    }

    void UpdateStartingPosition(Vector3 newPosition)
    {
        StartingPosition = newPosition;
        Debug.Log($"HYUNSOO, received new position: {StartingPosition}");
    }


    // UI handlers
    void StartAutoSimHandler()
    {
        // Reset positions, hyperparams
        CurrentInteraction.VirtObj.GetComponent<Transform>().position = StartingPosition;
        CurrentInteraction.ResetParams();
        trajectory.Clear();
        isSimulating = true;
        CurrentInteraction.StartSimulation();
    }

    void StopAutoSimHandler()
    {
        CurrentInteraction.StopSimulation();
        isSimulating = false;
        SimulationUtils.SaveToFile(SimulatedFilePath, "timestamp,x,y,z", trajectory);
    }

    void ChangeInteractionHandler(int val)
    {
        // Destroy old interaction component
        if (CurrentInteraction is MonoBehaviour oldInteraction)
        {
            Destroy(oldInteraction);
        }

        switch ((InteractionType)val)
        {
            case InteractionType.Pz_Bounciness_FreeFall:
                var freeFall = VirtualBall.AddComponent<PzFreeFall>();
                VirtualObject = VirtualBall.AddComponent<PzVirtualObject>();
                CurrentInteraction = freeFall;
                Debug.Log("HYUNSOO, freefall interaction");
                break;
            default:
                Debug.Log("HYUNSOO, default interaction");
                CurrentInteraction = null;
                break;
        }
    }

    void SearchMethodHandler(int val)
    {

    }
}
