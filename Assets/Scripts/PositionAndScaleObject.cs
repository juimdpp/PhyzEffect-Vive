using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionAndScaleObject : MonoBehaviour
{
    public delegate void PositionUpdated(Vector3 position);
    public static event PositionUpdated OnPositionUpdated;

    public GameObject RefCube1;
    public GameObject RefCube2;
    public GameObject RefCube3;
    public GameObject DeskCube1;
    public GameObject DeskCube2;
    public GameObject DeskCube3;

    public Button calibrateBtn;

    private Vector3 RefPos1;
    private Vector3 RefPos2;
    private Vector3 RefPos3;
    private Vector3 DeskPos1;
    private Vector3 DeskPos2;
    private Vector3 DeskPos3;

    // Start is called before the first frame update
    void Start()
    {
        calibrateBtn.onClick.AddListener(Recalibrate);
    }

    void Recalibrate()
    {
        // TODO: uncomment this!!
        /*RefPos1 = RefCube1.GetComponent<Transform>().position;
        RefPos2 = RefCube2.GetComponent<Transform>().position;
        RefPos3 = RefCube3.GetComponent<Transform>().position;
        DeskPos1 = DeskCube1.GetComponent<Transform>().position;
        DeskPos2 = DeskCube2.GetComponent<Transform>().position;
        DeskPos3 = DeskCube3.GetComponent<Transform>().position;

        // Match scale
        float refWidth = Mathf.Abs(RefPos1.x - RefPos2.x);
        float refHeight = Mathf.Abs(RefPos1.y - RefPos2.y);
        float refDepth = Mathf.Abs(RefPos2.z - RefPos3.z);
        float deskWidth = Mathf.Abs(DeskPos1.x - DeskPos2.x);
        float deskHeight = Mathf.Abs(DeskPos1.y - DeskPos2.y);
        float deskDepth = Mathf.Abs(DeskPos2.z - DeskPos3.z);

        
        transform.localScale = new Vector3(refWidth / deskWidth, refHeight / deskHeight, refDepth / deskDepth);

        Debug.Log("RefCube1: " + RefPos1 + " worldToCam: " + Camera.main.worldToCameraMatrix.MultiplyPoint(RefPos1));
        Debug.Log("RefCube2: " + RefPos2 + " worldToCam: " + Camera.main.worldToCameraMatrix.MultiplyPoint(RefPos2));
        Debug.Log("RefCube3: " + RefPos3 + " worldToCam: " + Camera.main.worldToCameraMatrix.MultiplyPoint(RefPos3));
        Debug.Log("Reference scales: " + refWidth + ", " + refHeight + ", " + refDepth);
        // Debug.Log("Desk scales: " + deskWidth + ", " + deskHeight + ", " + deskDepth);

        // Update position to the middle cube, but put it a bit in front and on top. Define bit as half the size of the arucoMarker (one of the RefCubes)
        float shiftOffset = RefCube2.GetComponent<Transform>().localScale.x;
        transform.position = RefPos2;*/

        OnPositionUpdated?.Invoke(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
