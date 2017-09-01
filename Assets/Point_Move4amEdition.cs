using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointNReel2 : SteamVR_TrackedController
{
    public static SteamVR_Camera headset;
    private       SteamVR_TrackedController controller;

    public Transform raycast;

    Ray ray;

    float acceleration;
    float deceleration;
    float decelMag;
    Vector3 direction;
    Vector3 sensitivity;
    Vector3 velocity;

    Vector3 pointOne;
    Vector3 pointTwo;

    // Use this for initialization
    void Start()
    {
        pointOne = Vector3.zero;
        pointTwo = Vector3.zero;

        decelMag = 0.25f;

        controller = GetComponentInParent<SteamVR_TrackedController>();

        headset = controller.GetComponentInParent<SteamVR_Camera>();
    }

    private void pointPressed() //When the point button is pressed.
    {
        ray = new Ray(raycast.transform.position, raycast.transform.forward);

        velocity = ray.direction;
    }

    private void gripPressed(Vector3 pointOne, Vector3 PointTwo)
    {
        pointOne = PointTwo;

        PointTwo = transform.localPosition;
    }

    private void calAcceration(Vector3 pointOne, Vector3 pointTwo)
    {
        acceleration = pointTwo.magnitude - pointOne.magnitude;
    }
    
    private void decelerate()
    {
        deceleration = acceleration * decelMag;

        velocity = velocity + new Vector3(0, 0, deceleration);
    }

    private void calVelocity(float acceleration)
    {
        velocity = velocity + new Vector3(0,0, acceleration);
    }

    private void moveHeadset()
    {
        headset.transform.position = headset.transform.position + velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.menuPressed)
        {
            pointPressed();
        }

        if (controller.gripped)
        {
            gripPressed(pointOne, pointTwo);
            calAcceration(pointOne, pointTwo);
            calVelocity(acceleration);
            moveHeadset();
        }

        if(controller.gripped == false)
        {
            decelerate();
        }

        else if (velocity.magnitude > 0)
        {
            calAcceration(pointOne, pointTwo);
            calVelocity(acceleration);
            moveHeadset();
        }
    }
}