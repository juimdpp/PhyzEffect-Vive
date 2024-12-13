using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using sl;

public class BallDetector : MonoBehaviour
{
    public Button button;
    public ZEDCamera zedCamera; // should be different from input zedcamera?? TODO: check
    public ZEDManager zedManager;
    public string svoInputFile;

    private InitParameters initParameters;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[HYUNSOO]");
        initParameters.pathSVO = svoInputFile;
        Debug.Log("[HYUNSOO] pathSVO: " + initParameters.pathSVO);
        zedCamera.Open(ref initParameters);
        
        Debug.Log("[HYUNSOO] zedCamera: " + zedCamera.ToString());
        if (!zedManager) zedManager = FindObjectOfType<ZEDManager>();
        Debug.Log("[HYUNSOO] Number of frames in video: " + zedManager.NumberFrameMax);


        if (zedManager.objectDetectionModel != OBJECT_DETECTION_MODEL.CUSTOM_BOX_OBJECTS)
        {
            Debug.LogWarning("sl.DETECTION_MODEL.CUSTOM_BOX_OBJECTS is mandatory for this sample");
        }
        //else
        //{
        //    //We'll listen for updates from a ZEDToOpenCVRetriever, which will call an event whenever it has a new image from the ZED. 
        //    if (!imageRetriever) imageRetriever = ZEDToOpenCVRetriever.GetInstance();
        //    imageRetriever.OnImageUpdated_LeftRGBA += Run;
        //}
        // Init();

        // Initialize button
        button.onClick.AddListener(PlayOrPause);
        button.GetComponentInChildren<TMP_Text>().text = zedManager.pauseSVOReading == false ? "Play" : "Pause";

    }

    void PlayOrPause()
    {
        zedManager.pauseSVOReading = !zedManager.pauseSVOReading;
        button.GetComponentInChildren<TMP_Text>().text = zedManager.pauseSVOReading == false ? "Play" : "Pause";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
