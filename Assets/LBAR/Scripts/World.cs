using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
    {
        public string licenseKeyAndroid;
        public string licenseKeyiOS;
        public float range = 5.0f;
        public float offsetBios = 10.0f; //10.0f
        public float distanceRatio = 50.0f; //500.0f
        public List<POI> poiList;
        public bool clustering = true;
        public int clusteringAngle = 30;
        public bool radarOn;
        private string licenseKey;
        private string privateKey;
        private LocationInfo mLastLocation;
        private bool locationInitialized = false;
        private bool camAvailable;
        WebCamTexture backCam;
        GameObject background;
        Camera mainCam;
        public bool test = true;

        Vector2 homeLoc;
        Vector2 mosqueLoc;
        Vector2 parkLoc;
        Vector2 pizzaLoc;

        //private Text debugger;

        public World(float _range = 0.5f, float _offsetBios = 10.0f, float _distanceRatio = 50.0f, bool _clustering = true, int _clusteringAngle = 30, bool _radarOn = true)
        {

            range = _range;
            offsetBios = _offsetBios;
            distanceRatio = _distanceRatio;
            clustering = _clustering;
            clusteringAngle = _clusteringAngle;
            radarOn = _radarOn;
            poiList = new List<POI>();


            Debug.Log("range: " + range + ",offsetBios: " + offsetBios + ",distanceRatio: " + distanceRatio + ",clusteringAngle: " + clusteringAngle);
        }

        void Reset()
        {
            range = 0.5f;
            offsetBios = 20.0f;
        }

        void Start()
        {


            homeLoc = new Vector2(35.715104f, 51.36221f);
            mosqueLoc = new Vector2(35.712935f, 51.361073f);
            parkLoc = new Vector2(35.716625f, 51.366885f);
            pizzaLoc = new Vector2(35.715236f, 51.362857f);

            World mWorld = new World();

            //debugger = GameObject.Find("debugger").GetComponent<Text>();



            //initCam();
            if (test)
            {
                updatePOIs(poiList);
            }
            else
            {
                StartCoroutine(getLocation());
            }


        }



        

        void Update()
        {

            worldUpdate();
        }

        public void worldUpdate()
        {
            //updateCam();

            //debugger.text = "lastLoc: (" + mLastLocation.latitude + " , " + mLastLocation.longitude + ")";
            if ((Input.location.lastData.latitude != mLastLocation.latitude && Input.location.lastData.longitude != mLastLocation.longitude) && locationInitialized)
            {
                mLastLocation = Input.location.lastData;
                updatePOIs(this.poiList);
            }


        }

        private IEnumerator getLocation()
        {


            if (!Input.location.isEnabledByUser)
            {
                yield break;
            }

            Input.location.Start();
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                print("Timed Out");
                yield break;
            }

            //Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {
                //Access granted and location value could be retrieved
                mLastLocation = Input.location.lastData;
                updatePOIs(poiList);
                locationInitialized = true;

            }

        }

        public void updatePOIs(List<POI> mPOIs)
        {
            foreach (POI poi in mPOIs)
            {
                poi.gameObject.transform.position = Vector3.zero;
                poi.gameObject.SetActive(false);
                if (poi.radarDot != null) poi.radarDot.transform.SetParent(null);
                float distance = distanceCal(poi.lat, poi.lon);
                Debug.Log("Range: " + range + ", distance: " + distance);
                if (distance > range)
                {
                    poi.gameObject.SetActive(false);
                }
                else
                {
                    poi.gameObject.SetActive(true);
                }

                float mAngleCal = angleCal(poi.lat, poi.lon);
                poi.gameObject.transform.eulerAngles = new Vector3(0, mAngleCal, 0);
                if (poi.radarDot != null) poi.radarDot.transform.eulerAngles = new Vector3(0, mAngleCal, 0);
                float mDistance = distanceCal(poi.lat, poi.lon);
                Vector3 radarOffset = Vector3.zero;
                if (poi.radarDot != null) radarOffset = (poi.radarDot.transform.forward * distanceRatio * (Mathf.Sqrt(mDistance * 1000) / 100));
                Vector3 offset = (poi.gameObject.transform.forward * offsetBios) + (poi.gameObject.transform.forward * distanceRatio * (Mathf.Sqrt(mDistance * 1000) / 100));
                offset = new Vector3(offset.x, poi.height, offset.z);
                Vector3 total = poi.gameObject.transform.position + offset;

                poi.gameObject.transform.position = Vector3.zero + offset;
                poi.distance = mDistance;
                if (poi.radarDot != null)
                {
                    Debug.Log("radarOffset: " + radarOffset);
                    poi.radarDot.transform.position = Vector3.zero + radarOffset;// poi.radarDot.transform.localPosition + radarOffset;
                    poi.radarDot.transform.SetParent(poi.gameObject.transform);
                }


            }

            if (clustering)
            {
                clusterPOIs(clusteringAngle);
            }
        }

        public void addPOI(POI mPOI)
        {
            poiList.Add(mPOI);
        }

        public void clearPOIs()
        {
            poiList.Clear();
        }

        public float distanceCal(float lat, float lon) //in KM
        {
            float lat1;
            float lon1;
            if (!test)
            {
                lat1 = Input.location.lastData.latitude;
                lon1 = Input.location.lastData.longitude;
            }
            else
            {
                lat1 = homeLoc.x;
                lon1 = homeLoc.y;
            }

            var R = 6378.137; // Radius of earth in KM
            var dLat = lat * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
            var dLon = lon * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
            float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat * Mathf.PI / 180) *
                Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
            var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            var d = (float)(R * c);
            //d= d * 1000f; // meters

            return d;

        }

        public float angleCal(float lat, float lon) //returns in Degrees
        {

            float lat1;
            float long1;
            float dLon;
            if (!test)
            {
                lat1 = Input.location.lastData.latitude;
                long1 = Input.location.lastData.longitude;
                dLon = (lon - long1);
            }
            else
            {
                lat1 = homeLoc.x;
                long1 = homeLoc.y;
                dLon = (lon - long1);
            }

            float y = Mathf.Sin(dLon) * Mathf.Cos(lat);
            float x = Mathf.Cos(lat1) * Mathf.Sin(lat) - Mathf.Sin(lat1) * Mathf.Cos(lat) * Mathf.Cos(dLon);

            float brng = Mathf.Atan2(y, x);

            brng = brng * Mathf.Rad2Deg;
            brng = 360 - brng;
            brng = brng % 360;

            return brng;
        }

        public void clusterPOIs(int angle)
        {
            GameObject[] POIs = new GameObject[poiList.Count];
            for (int i = 0; i < poiList.Count; i++)
            {
                POIs[i] = poiList[i].gameObject;
            }
            int totalClusters = 360 / angle;
            List<GameObject>[] poiAngles = new List<GameObject>[totalClusters];
            for (int j = 0; j < totalClusters; j++)
            {
                poiAngles[j] = new List<GameObject>();
                for (int i = 0; i < POIs.Length; i++)
                {
                    if (POIs[i].transform.eulerAngles.y > j * angle && POIs[i].transform.eulerAngles.y < (j + 1) * angle)
                    {
                        Debug.Log("between (" + j * angle + " , " + (j + 1) * angle + ")");
                        poiAngles[j].Add(POIs[i]);
                    }
                }
                float yOffset = 0.5f;
                for (int k = 0; k < poiAngles[j].Count; k++)
                {

                    yOffset = Mathf.Pow(-1, k) * Mathf.Floor((k + 1) / 2) * 6 + UnityEngine.Random.Range(-2, 2);

                    if (poiAngles[j][k].GetComponent<POI>().height == 0)
                        poiAngles[j][k].transform.position = new Vector3(poiAngles[j][k].transform.position.x, yOffset, poiAngles[j][k].transform.position.z);
                    else
                        poiAngles[j][k].transform.position = new Vector3(poiAngles[j][k].transform.position.x, poiAngles[j][k].GetComponent<POI>().height, poiAngles[j][k].transform.position.z);
                }
            }
        }



    }