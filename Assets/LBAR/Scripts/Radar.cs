using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radar : MonoBehaviour
{

    [HideInInspector]
    public GameObject radarCamObj;
    [HideInInspector]
    public bool radarInitiated = false;

    public Radar()
    {
    }

    void Start()
    {

        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            return;


        radarCamObj = new GameObject();
        radarCamObj.name = "radarCamObj";
        radarCamObj.AddComponent<Camera>();
        Camera radarCam = radarCamObj.GetComponent<Camera>();
        //Instantiate(radarCam);
        radarCam.transform.position = new Vector3(0, 50.0f, 0);
        radarCam.transform.eulerAngles = new Vector3(90, 0, 0);
        radarCam.orthographic = true;
        radarCam.orthographicSize = 40;
        radarCam.nearClipPlane = 0.3f;
        radarCam.farClipPlane = 1000;
        radarCam.depth = 1;

        RenderTexture radarRT = new RenderTexture(512, 512, 16);
        radarCam.targetTexture = radarRT;
        radarCamObj.GetComponent<Camera>().cullingMask = 1 << LayerMask.NameToLayer("radar");
        radarCamObj.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        GameObject mCanvas = GameObject.Find("Canvas");
        if (mCanvas == null)
            return;

        GameObject radarObj = GameObject.Find("Radar");
        GameObject radarPlane = GameObject.Find("RadarPlane");
        radarObj.transform.SetAsLastSibling();
        radarPlane.GetComponent<RawImage>().texture = radarRT;

        radarInitiated = true;

    }

    void Update()
    {
        if (LBARCam.wikitudeModeInited)
        {
            if (LBARCam.wikitudeMode)
            {
                if (LBARCam.secondCam != null)
                    radarCamObj.transform.eulerAngles = new Vector3(90, LBARCam.secondCam.transform.eulerAngles.y, 0);
            }
            else
            {
                radarCamObj.transform.eulerAngles = new Vector3(90, Camera.main.transform.eulerAngles.y, 0);
            }
        }
    }
}