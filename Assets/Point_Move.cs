using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Expiremental Point Move Locomotion
namespace VRTK
{
    public class Point_Move : VRTK_ObjectControlPointMove
    {

        Vector3 acceleration;
        Vector3 deceleration;
        Vector3 decelMag;
        Vector3 direction;
        Vector3 sensitivity;
        Vector3 velocity;

        Vector3 headsetEyePosition;
        Vector3 headsetEyeGroundPosition;
        Vector3 headsetRelativeRig;
        Vector3 startPosition;

        Ray ray;

        // Use this for initialization
        void Start()
        {

        }




        // Update is called once per frame
        void Update()
        {

        }


        //VRTK STUFF

        //[Tooltip("An optional button that has to be engaged to allow the touchpad control to activate.")]
        //public VRTK_ControllerEvents.ButtonAlias primaryActivationButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Touch;
        //[Tooltip("An optional button that when engaged will activate the modifier on the touchpad control action.")]
        //public VRTK_ControllerEvents.ButtonAlias actionModifierButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;

        [Header("Point and Move Control Settings")]

        [Tooltip("An optional button that has to be engaged to allow the touchpad control to activate.")]
        public VRTK_ControllerEvents.ButtonAlias primaryActivationButton = VRTK_ControllerEvents.ButtonAlias.Button_One_Press;
        [Tooltip("An optional button that when engaged will activate the modifier on the touchpad control action.")]
        public VRTK_ControllerEvents.ButtonAlias actionModifierButton    = VRTK_ControllerEvents.ButtonAlias.Grip_Press      ;

        protected bool dragFirstChange             ;
        protected bool otherGripControlEnabledState;

        protected override void OnEnable()
        {
            base.OnEnable();
            dragFirstChange = true;
        }

        protected override void ControlFixedUpdate()
        {
            ModifierButtonActive();
        }

        protected override VRTK_ObjectControlPointMove GetOtherControl()
        {
            GameObject foundController = (VRTK_DeviceFinder.IsControllerLeftHand(gameObject) ? VRTK_DeviceFinder.GetControllerRightHand(false) : VRTK_DeviceFinder.GetControllerLeftHand(false));
            if (foundController)
            {
                return foundController.GetComponent</*VRTK_ButtonControl*/VRTK_ObjectControlPointMove>();
            }
            return null;
        }

        protected override void SetListeners(bool state)
        {
            if (controllerEvents)
            {
                if (state)
                {
                    //controllerEvents.TouchpadAxisChanged += TouchpadAxisChanged;
                    //controllerEvents.TouchpadTouchEnd    += TouchpadTouchEnd;

                    controllerEvents.GripPressed  += GripPressed ;
                    controllerEvents.GripTouchEnd += GripTouchEnd;
                }
                else
                {
                    //controllerEvents.TouchpadAxisChanged -= TouchpadAxisChanged;
                    //controllerEvents.TouchpadTouchEnd    -= TouchpadTouchEnd;

                    controllerEvents.GripPressed  += GripPressed ;
                    controllerEvents.GripTouchEnd += GripTouchEnd;

                }
            }
        }

        protected override bool IsInAction()
        {
            return (ValidPrimaryButton() && Button_One_Press());
        }

        //protected virtual bool OutsideDeadzone(float axisValue, float deadzoneThreshold)
        //{
        //    return (axisValue > deadzoneThreshold || axisValue < -deadzoneThreshold);
        //}

        protected virtual bool ValidPrimaryButton()
        {
            return (controllerEvents && (primaryActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined || controllerEvents.IsButtonPressed(primaryActivationButton)));
        }

        protected virtual void ModifierButtonActive()
        {
            modifierActive = (controllerEvents && actionModifierButton != VRTK_ControllerEvents.ButtonAlias.Undefined && controllerEvents.IsButtonPressed(actionModifierButton));
        }

        protected virtual bool Button_One_Press()
        {
            return (controllerEvents && controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.Button_One_Press));
        }

        protected virtual void GripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (dragFirstChange && otherObjectControl && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
            {
                otherGripControlEnabledState = otherObjectControl.enabled;
                otherObjectControl.enabled = false;
            }
            currentAxis = (ValidPrimaryButton() ? e.touchpadAxis : Vector2.zero);

            if (currentAxis != Vector2.zero)
            {
                dragFirstChange = false;
            }
        }

        protected virtual void GripTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (otherObjectControl && disableOtherControlsOnActive)
            {
                otherObjectControl.enabled = otherGripControlEnabledState;
            }
            currentAxis = Vector2.zero;
            dragFirstChange = true;
        }

        //protected virtual void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        //{
        //    if (dragFirstChange && otherObjectControl && disableOtherControlsOnActive && e.touchpadAxis != Vector2.zero)
        //    {
        //        otherTouchpadControlEnabledState = otherObjectControl.enabled;
        //        otherObjectControl.enabled = false;
        //    }
        //    currentAxis = (ValidPrimaryButton() ? e.touchpadAxis : Vector2.zero);

        //    if (currentAxis != Vector2.zero)
        //    {
        //        dragFirstChange = false;
        //    }
        //}

        //protected virtual void TouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        //{
        //    if (otherObjectControl && disableOtherControlsOnActive)
        //    {
        //        otherObjectControl.enabled = otherTouchpadControlEnabledState;
        //    }
        //    currentAxis = Vector2.zero;
        //    dragFirstChange = true;
        //}
    }
}


