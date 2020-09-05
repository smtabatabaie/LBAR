using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{

    public int id;
    public float lat;
    public float lon;
    public float height;
    public string metadata;
    [HideInInspector]
    public float distance;
    [HideInInspector]
    public bool initialized = false;
    [HideInInspector]
    public GameObject radarDot;

    public POI(int _id, float _lat, float _lon, float _height = 0, string _metadata = "")
    {
        //POIObj = _POIObj;
        id = _id;
        lat = _lat;
        lon = _lon;
        metadata = _metadata;
        height = _height;

    }


    void Start()
    {

        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            return;


        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(3, 3, 3);

        //check if there are any Radar objects in the scene to generate the radarDot
        Radar[] radarObj = GameObject.FindObjectsOfType<Radar>();
        if (radarObj.Length > 0)
        {
            radarDot = sphere;
            radarDot.layer = LayerMask.NameToLayer("radar");
            radarDot.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Color");
            radarDot.GetComponent<MeshRenderer>().material.color = Color.white;
            Destroy(radarDot.GetComponent<SphereCollider>());
            radarDot.name = "radarDot";

            //radarDot = Instantiate(radarDot);
            radarDot.transform.SetParent(this.gameObject.transform);
            this.gameObject.SetActive(false);
            Debug.Log("inactiving poi : " + this.gameObject.name);
            initialized = true;
        }
    }



}