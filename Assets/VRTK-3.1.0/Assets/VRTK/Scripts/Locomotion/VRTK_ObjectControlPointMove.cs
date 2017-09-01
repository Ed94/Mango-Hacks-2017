// Object Control|Locomotion|20060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controlledGameObject">The GameObject that is going to be affected.</param>                             //Play area?
    /// <param name="directionDevice">The device that is used for the direction.</param>                                    //Whatever will emit the raycast.
    /// <param name="axisDirection">The axis that is being affected.</param>                                                //Z direction of ray?
    /// <param name="axis">The value of the current touchpad touch point based across the axis direction.</param>           //Used for ray?
    /// <param name="deadzone">The value of the deadzone based across the axis direction.</param>                           //Not needed for point move.
    /// <param name="currentlyFalling">Whether the controlled GameObject is currently falling.</param>
    /// <param name="modifierActive">Whether the modifier button is pressed.</param>
    public struct ObjectControlPointMoveEventArgs
    {
        public GameObject controlledGameObject;
        public Transform  directionDevice;
        public Vector3    axisDirection;
        public float      axis;
        public float      deadzone;
        public bool       currentlyFalling;
        public bool       modifierActive;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ObjectControlPointMoveEventArgs"/></param>
    public delegate void ObjectControlPointMoveEventHandler(object sender, ObjectControlPointMoveEventArgs e);

    /// <summary>
    /// An abstract class to provide a mechanism to control an object based on controller input.
    /// </summary>
    /// <remarks>
    /// As this is an abstract class, it cannot be applied directly to a game object and performs no logic.
    /// </remarks>
    public abstract class VRTK_ObjectControlPointMove : MonoBehaviour
    {
        /// <summary>
        /// Devices for providing direction.
        /// </summary>
        /// <param name="Headset">The headset device.</param>
        /// <param name="LeftController">The left controller device.</param>
        /// <param name="RightController">The right controller device.</param>
        /// <param name="ControlledObject">The controlled object.</param>
        public enum DirectionDevices
        {
            Headset,
            LeftController,
            RightController,
            ControlledObject
        }

        [Header("Control Settings")]

        [Tooltip("The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same GameObject.")]
        public VRTK_ControllerEvents controller;
        [Tooltip("The direction that will be moved in is the direction of this device.")]
        public DirectionDevices deviceForDirection = DirectionDevices.Headset;
        [Tooltip("If this is checked then whenever the axis on the attached controller is being changed, all other object control scripts of the same type on other controllers will be disabled.")]
        public bool disableOtherControlsOnActive = true;
        [Tooltip("If a `VRTK_BodyPhysics` script is present and this is checked, then the object control will affect the play area whilst it is falling.")]
        public bool affectOnFalling = false;
        [Tooltip("An optional game object to apply the object control to. If this is blank then the PlayArea will be controlled.")]
        public GameObject controlOverrideObject;

        /// <summary>
        /// Emitted when the X Axis Changes.
        /// </summary>
        public event ObjectControlPointMoveEventHandler GripPressed; //Change to Grip pressed?

        /// <summary>
        /// Emitted when the Y Axis Changes.
        /// </summary>
        public event ObjectControlPointMoveEventHandler GripReleased; //Changed to menu pressed?

        protected VRTK_ControllerEvents       controllerEvents          ;
        protected VRTK_BodyPhysics            bodyPhysics               ;
        protected VRTK_ObjectControlPointMove otherObjectControl        ;
        protected GameObject                  controlledGameObject      ;
        protected GameObject                  setControlOverrideObject  ;
        protected Transform                   directionDevice                            ;
        protected DirectionDevices            previousDeviceForDirection                 ;
        protected Vector2                     currentAxis                                ;
        protected Vector2                     storedAxis                                 ;
        protected bool                        currentlyFalling = false                   ;
        protected bool                        modifierActive   = false                   ;
        protected float                       controlledGameObjectPreviousY       = 0f   ;
        protected float                       controlledGameObjectPreviousYOffset = 0.01f;

        public virtual void onGripPressed(ObjectControlPointMoveEventArgs e)
        {
            if (GripPressed != null)
            {
                GripPressed(this, e);
            }
        }

        public virtual void onGripReleased(ObjectControlPointMoveEventArgs e)
        {
            if (GripReleased != null)
            {
                GripReleased(this, e);
            }
        }

        protected abstract void ControlFixedUpdate();
        protected abstract VRTK_ObjectControlPointMove GetOtherControl();
        protected abstract bool IsInAction();
        protected abstract void SetListeners(bool state);

        protected virtual void OnEnable()
        {
            currentAxis = Vector2.zero; //change to raycast?
            storedAxis = Vector2.zero; //change to raycast saved after press?
            controllerEvents = (controller != null ? controller : GetComponent<VRTK_ControllerEvents>());
            if (!controllerEvents)
            {
                Debug.LogError("A `VRTK_ControllerEvents` script is required for the `VRTK_ObjectControl` script to work. Either the `controller` parameter is not set or no `VRTK_ControllerEvents` is attached to this GameObject.");
                return;
            }
            SetControlledObject();
            bodyPhysics = (!controlOverrideObject ? FindObjectOfType<VRTK_BodyPhysics>() : null);

            directionDevice = GetDirectionDevice();
            SetListeners(true);
            otherObjectControl = GetOtherControl();
        }

        protected virtual void OnDisable()
        {
            SetListeners(false);
        }

        protected virtual void Update()
        {
            if (controlOverrideObject != setControlOverrideObject)
            {
                SetControlledObject();
            }
        }

        protected virtual void FixedUpdate()
        {
            CheckDirectionDevice();
            //CheckFalling        ();
            ControlFixedUpdate  ();
        }

        protected virtual ObjectControlPointMoveEventArgs SetEventArguements(Vector3 axisDirection, float axis, float axisDeadzone)
        {
            ObjectControlPointMoveEventArgs e;
            e.controlledGameObject = controlledGameObject;
            e.directionDevice = directionDevice;
            e.axisDirection = axisDirection;
            e.axis = axis;
            e.deadzone = axisDeadzone;
            e.currentlyFalling = currentlyFalling;
            e.modifierActive = modifierActive;

            return e;
        }

        protected virtual void SetControlledObject()
        {
            setControlOverrideObject      = controlOverrideObject;
            controlledGameObject          = (controlOverrideObject ? controlOverrideObject : VRTK_DeviceFinder.PlayAreaTransform().gameObject);
            controlledGameObjectPreviousY = controlledGameObject.transform.position.y; //Area of Interest.
        }

        //protected virtual void CheckFalling()
        //{
        //    if (bodyPhysics && bodyPhysics.IsFalling() && ObjectHeightChange())
        //    {
        //        if (!affectOnFalling)
        //        {
        //            if (storedAxis == Vector2.zero)
        //            {
        //                storedAxis = new Vector2(currentAxis.x, currentAxis.y);
        //            }
        //            currentAxis = Vector2.zero;
        //        }
        //        currentlyFalling = true;
        //    }

        //    if (bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
        //    {
        //        currentAxis = (IsInAction() ? storedAxis : Vector2.zero);
        //        storedAxis = Vector2.zero;
        //        currentlyFalling = false;
        //    }
        //}

        //protected virtual bool ObjectHeightChange()
        //{
        //    bool heightChanged = ((controlledGameObjectPreviousY - controlledGameObjectPreviousYOffset) > controlledGameObject.transform.position.y);
        //                         controlledGameObjectPreviousY = controlledGameObject.transform.position.y;

        //    return heightChanged;
        //}

        protected virtual Transform GetDirectionDevice()
        {
            switch (deviceForDirection)
            {
                case DirectionDevices.ControlledObject:
                    return controlledGameObject.transform;
                case DirectionDevices.Headset:
                    return VRTK_DeviceFinder.HeadsetTransform();
                case DirectionDevices.LeftController :
                    return VRTK_DeviceFinder.GetControllerLeftHand (true).transform;
                case DirectionDevices.RightController:
                    return VRTK_DeviceFinder.GetControllerRightHand(true).transform;
            }

            return null;
        }

        protected virtual void CheckDirectionDevice()
        {
            if (previousDeviceForDirection != deviceForDirection)
            {
                directionDevice = GetDirectionDevice();
            }

            previousDeviceForDirection = deviceForDirection;
        }
    }
}