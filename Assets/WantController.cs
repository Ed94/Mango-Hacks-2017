using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : SteamVR_TrackedController
{
    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)controllerIndex); } }
    public Vector3 velocity { get { return controller.velocity; } }
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }

    public Vector3 usedVel;

    Vector3 direction;
    public Transform raycast;

    public void Start()
    {
        //base.Start();
    }

    public void Update()
    {
        //base.Update();
    }

    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);
    }

    public override void OnMenuClicked(ClickedEventArgs e)
    {
        base.OnMenuClicked(e);

        Ray ray = new Ray(raycast.transform.position, raycast.transform.forward);

        direction = raycast.transform.forward;

        direction = velocity;
    }

    public override void OnMenuUnclicked(ClickedEventArgs e)
    {
        base.OnMenuUnclicked(e);
    }

    public override void OnSteamClicked(ClickedEventArgs e)
    {
        base.OnSteamClicked(e);
    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);
    }

    public override void OnPadTouched(ClickedEventArgs e)
    {
        base.OnPadTouched(e);
    }

    public override void OnPadUntouched(ClickedEventArgs e)
    {
        base.OnPadUntouched(e);
    }

    public override void OnGripped(ClickedEventArgs e)
    {
        base.OnGripped(e);

        usedVel = new Vector3(0, 0, velocity.z);
        usedVel = usedVel + direction;

        transform.parent.position = transform.parent.position + usedVel;
    }

    public override void OnUngripped(ClickedEventArgs e)
    {
        base.OnUngripped(e);
    }
}