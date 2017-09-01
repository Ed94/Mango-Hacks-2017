using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointNReal : MonoBehaviour
    {
        public GameObject cameraRig;

        private SteamVR_Camera headset;

        public Transform raycast;

        double velocity;
        double acceleration;

        Vector3 pointOne;
        Vector3 pointTwo;

        // Use this for initialization
        void Start()
        {
            headset = cameraRig.GetComponent<SteamVR_Camera>(); //Get the headset.
        }

        private void pointPressed() //When the point button is pressed.
        {
            Ray ray = new Ray(raycast.transform.position, raycast.transform.forward);
        }

        private void gripPressed()
        {
            //Gets samples points
        }

        private void calAcceration(Vector3 pointOne, Vector3 pointTwo)
        {
            //uses sample points to calculate acceleration
            acceleration = 0;
        }

        private void calVelocity(double acceleration)
        {
            velocity = velocity + acceleration - 1;
        }

        private void moveHeadset()
        {
            //headset.
        }

        // Update is called once per frame
        void Update()
        {
            if (velocity > 0)
            {
                calAcceration(pointOne, pointTwo);
                calVelocity(acceleration);
            }
            moveHeadset();
        }
    }