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

    private bool notified = false;

    void Update()
    {
        if (gameObject.GetComponent<Rigidbody>().IsSleeping())
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
    Vector3 StartingPosition { get; set; }
    
    public delegate void Event();
    public event Event OnEndAllSimulations;

    void Initialize(Vector3 _StartingPosition);
    void ResetParams();
    void UpdateParams();
    void ResetPosition();
    void StartAllSimulations();
    void StopAllSimulations();
    void StartSingleSimulation();
    void StopSingleSimulation();
}

// Makes GameObject to which it is attached to able to automatically simulate interaction (start - stop - update params)
public class PzFreeFall: MonoBehaviour, PzInteraction{
    public GameObject RealObj { get; set; }
    public PzVirtualObject VirtObj { get; set; }
    public Vector3 StartingPosition { get; set; }

    // Event
    public delegate void Event();
    public event PzInteraction.Event OnEndAllSimulations;

    private float gridStep = 0.1f; // TODO: make this controllable by PzOptimizer;

    private List<(float, float)> ParamsCombinations = new List<(float, float)>();
    private int currParamIdx = 0;
    private bool CanRun = false;
    
    void Awake()
    {
        VirtObj = GetComponent<PzVirtualObject>();
    }
    void Start()
    {
       VirtObj.OnRest += StopSingleSimulation;
    }
    public void Initialize(Vector3 _StartingPosition)
    {
        StartingPosition = _StartingPosition;
    }
    public void ResetParams()
    {
        Debug.Log("Reset parameters");
    }
    public void StartAllSimulations()
    {
        CanRun = true;
        Debug.Log("Start Auto Simulation (for freefall, start gravity, for friction, push motion)");

        // FreeFall has 2 parameters: VirtObj.bounciness, RealObj.bounciness. 
        // Create all possible combinations of the parameters using gridstep
        for(float r = 0.0f; r<=1; r+=gridStep)
        {
            for(float v = 0.0f; v<=1; v += gridStep)
            {
                ParamsCombinations.Add((r, v));
            }
        }

        UpdateParams();
        StartSingleSimulation();
        // TODO: params
    }
    public void StopAllSimulations()
    {
        // Debug.Log("Stop All Simulations (for freefall, stop gravity, for friction, stop motion)");
        OnEndAllSimulations?.Invoke();
        
    }

    public void StartSingleSimulation()
    {
        if (!CanRun) return;
        Debug.Log("StartSingleSimulation");
        VirtObj.GetComponent<Rigidbody>().useGravity = true;
    }
    public void StopSingleSimulation()
    {
        if (!CanRun) return;
        Debug.Log("StopSingleSimulation");
        VirtObj.GetComponent<Rigidbody>().useGravity = false;
        if(currParamIdx >= 0 && currParamIdx < ParamsCombinations.Capacity)
        {
            UpdateParams();
            ResetPosition();
            StartSingleSimulation();
        }
        else
        {
            StopAllSimulations();
        }
    }

    public void ResetPosition()
    {
        Debug.Log("ResetPosition to " + StartingPosition);
        VirtObj.GetComponent<Transform>().position = StartingPosition;
    }

    public void UpdateParams()
    {
        var tuple = ParamsCombinations[currParamIdx];
        Debug.Log("UpdateParams to " + tuple);
        VirtObj.GetComponent<Collider>().material.bounciness = tuple.Item1;
        RealObj.GetComponent<Collider>().material.bounciness = tuple.Item2;
        currParamIdx++;
    }

    // void ReturnTrajectory();
  
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
    // public GameObject VirtualBox; // Access VirtualObject via CurrentInteraction.VirtObj;
    private PzVirtualObject VirtualObject;
    public GameObject RealObject;
    // User inputs (can be changed to private)
    public string SimulatedFilePath;

    // Interactions
    private PzInteraction CurrentInteraction;

    // Other private stuff
    private Vector3 StartingPosition;
    private SortedDictionary<double, Vector3> trajectory = new SortedDictionary<double, Vector3>();
    private bool isSimulating;

    // Start is called before the first frame update
    void Start()
    {
        // Make the VirtualBall and VirtualBox PzInteractables
        VirtualBall.AddComponent<PzVirtualObject>();
        // VirtualBox.AddComponent<PzVirtualObject>();


        // UI handlers
        StartAutoSimBtn.onClick.AddListener(StartAutoSimHandler);
        StopAutoSimBtn.onClick.AddListener(StopAutoSimHandler);
        ChangeInteractionDpdwn.onValueChanged.AddListener(ChangeInteractionHandler);
        SearchMethodDpdwn.onValueChanged.AddListener(SearchMethodHandler);

        // Event handlers
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
        // TODO change below to newPosition
        StartingPosition = VirtualObject.GetComponent<Transform>().position;
        CurrentInteraction.StartingPosition = StartingPosition;
        Debug.Log($"HYUNSOO, received new position: {StartingPosition}");
    }


    // UI handlers
    void StartAutoSimHandler()
    {
        // Reset positions, hyperparams
        VirtualObject.GetComponent<Transform>().position = StartingPosition;
        CurrentInteraction.ResetParams();
        trajectory.Clear();
        isSimulating = true;
        CurrentInteraction.StartAllSimulations();
    }

    void StopAutoSimHandler()
    {
        // CurrentInteraction.StopSingleSimulation();
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
                // Event handler: Triggered when all possible combination of parameters are simulated
                CurrentInteraction.OnEndAllSimulations += StopAutoSimHandler;
                CurrentInteraction.RealObj = RealObject;
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

    private void OnApplicationQuit()
    {
        SimulationUtils.SaveToFile(SimulatedFilePath, "timestamp,x,y,z", trajectory);
    }
}
