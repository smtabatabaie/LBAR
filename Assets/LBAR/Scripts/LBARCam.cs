using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LBARCam : MonoBehaviour
{
    private GameObject background;
    private bool camAvailable;
    private WebCamTexture backCam;
    private Camera mainCam;
    private AspectRatioFitter backgroundAspectRatioFitter;
    private RectTransform backgroundRectTransform;

    public static bool wikitudeMode;
    public static Camera secondCam;
    public static bool wikitudeModeInited = false;

    void Start()
    {
        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            return;

        //if (this.GetComponent<WikitudeCamera>() == null)
        //    return;

        /*if (this.GetComponent<WikitudeCamera>() == null)
        {
            wikitudeMode = false;
            initCam2();
        }
        else
        {
            wikitudeMode = true;
            initCam();
        }*/


        if (GameObject.Find("WikitudeCamera") == null)
        {
            wikitudeMode = false;
            initCam2();
        }
        else
        {
            wikitudeMode = true;
            initCam();
        }

        //initCam();
    }

    void Update()
    {
        updateCam();
    }

    public void initCam()
    {
        wikitudeModeInited = true;
        //Creating second cam on top of wikitude
        GameObject secondCamObj = new GameObject();
        secondCamObj.AddComponent<Camera>();
        secondCam = secondCamObj.GetComponent<Camera>();
        //secondCamObj.tag = "secondCamObj";
        secondCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;



        Debug.Log("initing cam");
        background = new GameObject();
        background.AddComponent<RawImage>();
        background.AddComponent<AspectRatioFitter>();
        background.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        background.GetComponent<AspectRatioFitter>().aspectRatio = 0.8351852f;
        //GameObject mCanvas = GameObject.Find("Canvas");

        GameObject mCanvas;
        //creating canvas
        if (GameObject.Find("CanvasLBAR") == null)
        {
            mCanvas = new GameObject();
            mCanvas.AddComponent<Canvas>();
            mCanvas.AddComponent<CanvasScaler>();
            mCanvas.AddComponent<GraphicRaycaster>();
            mCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            mCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            mCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mCanvas.name = "CanvasLBAR";
            mCanvas.GetComponent<Canvas>().sortingOrder = -1;
        }
        else
        {
            mCanvas = GameObject.Find("CanvasLBAR");
        }


        if (mCanvas == null)
            return;
        background.transform.SetParent(mCanvas.transform);
        background.transform.SetAsFirstSibling();
        background.transform.localScale = new Vector3(1, 1, 1);
        background.GetComponent<RawImage>().rectTransform.position = new Vector3(0, 0, 0);
        background.GetComponent<RawImage>().rectTransform.anchorMin = new Vector2(0, 0);
        background.GetComponent<RawImage>().rectTransform.anchorMax = new Vector2(1, 1);
        background.GetComponent<RawImage>().rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        background.GetComponent<RawImage>().rectTransform.offsetMin = new Vector2(0, 0);
        background.GetComponent<RawImage>().rectTransform.offsetMax = new Vector2(0, 0);


        Texture defaultBackground = background.GetComponent<RawImage>().texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No Camera detected");
            camAvailable = false;
            //return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            //if (!devices[i].isFrontFacing)
            //{
            backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            //}
        }
        mainCam = secondCam;// GameObject.Find("sampleCam").GetComponent<Camera>();  //Camera.main;
        mainCam.transform.position = Vector3.zero;
        Input.gyro.enabled = true;
        GameObject cameraParent = new GameObject("camParent");
        cameraParent = Instantiate(cameraParent);
        cameraParent.transform.position = mainCam.transform.position;
        mainCam.transform.parent = cameraParent.transform;
        cameraParent.transform.Rotate(Vector3.right, 90);

        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            //return;
        }

        //backCam.Play();
        //background.GetComponent<RawImage>().texture = backCam;

        Camera.main.GetComponent<Camera>().cullingMask = ~(1 << LayerMask.NameToLayer("radar"));
        secondCam.GetComponent<Camera>().cullingMask = ~(1 << LayerMask.NameToLayer("radar"));

        //changin Canvas render mode so that it can render POIs on top of camera overlay
        mCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        mCanvas.GetComponent<Canvas>().worldCamera = mainCam;

        camAvailable = true;

        Debug.Log("background name: " + background.gameObject.name);
        background.SetActive(false);

    }



    public void initCam2()
    {
        Debug.Log("initing cam 2");

        wikitudeModeInited = true;

        background = new GameObject();
        background.AddComponent<RawImage>();
        background.AddComponent<AspectRatioFitter>();
        background.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        background.GetComponent<AspectRatioFitter>().aspectRatio = 0.8351852f;
        GameObject mCanvas = GameObject.Find("Canvas");
        if (mCanvas == null)
            return;
        background.transform.SetParent(mCanvas.transform);
        background.transform.SetAsFirstSibling();
        background.transform.localScale = new Vector3(1, 1, 1);
        background.GetComponent<RawImage>().rectTransform.position = new Vector3(0, 0, 0);
        background.GetComponent<RawImage>().rectTransform.anchorMin = new Vector2(0, 0);
        background.GetComponent<RawImage>().rectTransform.anchorMax = new Vector2(1, 1);
        background.GetComponent<RawImage>().rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        background.GetComponent<RawImage>().rectTransform.offsetMin = new Vector2(0, 0);
        background.GetComponent<RawImage>().rectTransform.offsetMax = new Vector2(0, 0);


        Texture defaultBackground = background.GetComponent<RawImage>().texture;


        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No Camera detected");
            camAvailable = false;
            return;
        }
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }
        mainCam = Camera.main;
        mainCam.transform.position = Vector3.zero;
        Input.gyro.enabled = true;
        GameObject cameraParent = new GameObject("camParent");
        cameraParent = Instantiate(cameraParent);
        cameraParent.transform.position = mainCam.transform.position;
        mainCam.transform.parent = cameraParent.transform;
        cameraParent.transform.Rotate(Vector3.right, 90);

        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            //return;
        }

        backCam.Play();
        background.GetComponent<RawImage>().texture = backCam;

        Camera.main.GetComponent<Camera>().cullingMask = ~(1 << LayerMask.NameToLayer("radar"));

        //changin Canvas render mode so that it can render POIs on top of camera overlay
        mCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        mCanvas.GetComponent<Canvas>().worldCamera = mainCam;

        camAvailable = true;


        backgroundRectTransform = background.GetComponent<RawImage>().rectTransform;
        backgroundAspectRatioFitter = background.GetComponent<AspectRatioFitter>();
    }

    private void updateCam()
    {
        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            return;

        //update cam
        if (!camAvailable)
            return;
        float ratio = (float)backCam.width / (float)backCam.height;
        background.GetComponent<AspectRatioFitter>().aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;


        if (wikitudeModeInited)
        {
            if (wikitudeMode)
                background.GetComponent<RawImage>().rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            else
                backgroundRectTransform.localScale = new Vector3(1f, scaleY, 1f);


            int orient = -backCam.videoRotationAngle;
            if (wikitudeMode)
                background.GetComponent<RawImage>().rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            else
                backgroundRectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }


        Quaternion cameraRotation = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
        mainCam.transform.localRotation = cameraRotation;
        //Debug.Log("updating Cam");

    }

}