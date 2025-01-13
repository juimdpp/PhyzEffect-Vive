using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZEDDepthTemp : MonoBehaviour
{
    public Button ShowDepthButton;
    public ZEDManager zedManager;
    // Start is called before the first frame update
    void Start()
    {
        ShowDepthButton.onClick.AddListener(showDepth);
    }

    private void showDepth()
    {
        Debug.Log("Occlusion: " + zedManager.depthOcclusion);
        zedManager.changeDepthOcclusion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
