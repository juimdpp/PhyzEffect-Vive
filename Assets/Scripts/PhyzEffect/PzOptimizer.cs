
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Function: UI Interface to change Optimizer and InteractionType
 */

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
    public string SimulatedTrajectoryFilePath;
    public string[] RealTrajectoryFilePaths;

    // Interactions
    private PzInteraction CurrentInteraction;

    // Other private stuff
    private Vector3 StartingPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Make the VirtualBall and VirtualBox PzInteractables
        VirtualBall.AddComponent<PzVirtualObject>();
        VirtualBox.AddComponent<PzVirtualObject>();


        // UI handlers
        StartAutoSimBtn.onClick.AddListener(StartAutoSimHandler);
        StopAutoSimBtn.onClick.AddListener(StopAutoSimHandler);
        ChangeInteractionDpdwn.onValueChanged.AddListener(ChangeInteractionHandler);
        SearchMethodDpdwn.onValueChanged.AddListener(SearchMethodHandler);

        // Event handlers
        // Triggered after scanned scene is positioned and scaled
        PositionAndScaleObject.OnPositionUpdated += UpdateStartingPosition;
        
        // Initialize interaction type to FreeFall by default
        ChangeInteractionHandler(0);

    }


    // TODO: check if we can remove this since we're extracting real trajectory (from which we should normally get the starting position) in the optimizer code
    void UpdateStartingPosition(Vector3 newPosition)
    {
        if (VirtualObject)
        {
            StartingPosition = VirtualObject.GetComponent<Transform>().position;
            CurrentInteraction.StartingPosition = StartingPosition;
            CurrentInteraction.EndPosition = StartingPosition + new Vector3(0, 0, 1); // Used only in PzPush?
            Debug.Log($"HYUNSOO, received new position: {StartingPosition}");
        }
        else
        {
            Debug.Log("HYUNSOO: not updated");
        }
    }


    // UI handlers
    void StartAutoSimHandler()
    {
        CurrentInteraction.StartOptimization();
    }

    void StopAutoSimHandler()
    {
        CurrentInteraction.StopOptimization();
    }
    void HandleInfMovement()
    {
        Debug.Log("Abnormal movement detected. Stopping automatic simulation");
        StopAutoSimHandler();
    }

    void ChangeInteractionHandler(int val)
    {
        Debug.Log("ChangeInteractionHandler");
        // Destroy old interaction component
        if (CurrentInteraction is MonoBehaviour oldInteraction)
        {
            Destroy(oldInteraction);
        }

        switch ((PzInteractionType)val)
        {
            case PzInteractionType.Pz_Bounciness_FreeFall:
                var freeFall = VirtualBall.AddComponent<PzPSOOptimizer>();
                VirtualObject = VirtualBall.AddComponent<PzVirtualObject>();
                VirtualObject.OnInfMovement += HandleInfMovement;
                CurrentInteraction = freeFall;
                // Event handler: Triggered when all possible combination of parameters are simulated
                CurrentInteraction.OnEndAllSimulations += StopAutoSimHandler;
                CurrentInteraction.RealObj = RealObject;
                // Load real trajectory from file (default = FreeFall)
                // SimulationUtils.LoadFile(RealTrajectoryFilePaths[0], CurrentInteraction.realTrajectory);
                Debug.Log("HYUNSOO, freefall interaction");
                break;
            case PzInteractionType.Pz_Bounciness_Push:
                var push = VirtualBox.AddComponent<PzPush>();
                VirtualObject = VirtualBox.AddComponent<PzVirtualObject>();
                VirtualObject.OnInfMovement += HandleInfMovement;
                CurrentInteraction = push;
                // Event handler: Triggered when all possible combination of parameters are simulated
                CurrentInteraction.OnEndAllSimulations += StopAutoSimHandler;
                CurrentInteraction.RealObj = RealObject;
                // Load real trajectory from file (default = FreeFall)
                // LoadFile(RealTrajectoryFilePaths[1]);
                Debug.Log("HYUNSOO, push interaction");
                break;
            default:
                Debug.Log("HYUNSOO, default interaction");
                CurrentInteraction = null;
                break;
        }
    }

    
    void SearchMethodHandler(int val)
    {
        // TODO
    }
}
