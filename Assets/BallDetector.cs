using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using sl;

public class BallDetector : MonoBehaviour
{
    public Button button;
    
    public ZEDManager zedManager;
    public GameObject zed;
    public string svoInputFile;

    private InitParameters initParameters = new InitParameters();
    private  ZEDCamera zedCamera; // should be different from input zedcamera?? TODO: check

    // Start is called before the first frame update
    void Start()
    {
        
        //else
        //{
        //    //We'll listen for updates from a ZEDToOpenCVRetriever, which will call an event whenever it has a new image from the ZED. 
        //    if (!imageRetriever) imageRetriever = ZEDToOpenCVRetriever.GetInstance();
        //    imageRetriever.OnImageUpdated_LeftRGBA += Run;
        //}
        // Init();

        // Initialize button
        button.onClick.AddListener(PlayVideo);
        button.GetComponentInChildren<TMP_Text>().text = zedManager.pauseSVOReading == false ? "Play" : "Pause";

    }

    void PlayVideo()
    {
        Debug.Log("Before reset");
        zedManager.inputType = sl.INPUT_TYPE.INPUT_TYPE_SVO;
        zedManager.svoInputFileName = "GitIgnoredFiles/Recording3.svo2";

        zedManager.Reset();
        Debug.Log("After reset");

        zedCamera = zedManager.zedCamera;
        Debug.Log("[HYUNSOO] Number of frames in video: " + zedCamera.GetSVONumberOfFrames());


        if (zedManager.objectDetectionModel != OBJECT_DETECTION_MODEL.CUSTOM_BOX_OBJECTS)
        {
            Debug.LogWarning("sl.DETECTION_MODEL.CUSTOM_BOX_OBJECTS is mandatory for this sample");
        }


        zedManager.pauseSVOReading = !zedManager.pauseSVOReading;
        button.GetComponentInChildren<TMP_Text>().text = zedManager.pauseSVOReading == false ? "Play" : "Pause";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
