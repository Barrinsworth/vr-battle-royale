using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public class OculusControllerInput : MonoBehaviour, IControllerInput
    {
        private const float THUMBSTICK_DEADZONE = 0.1f;

        [Header("--Oculus Controller Input--")]
        [SerializeField]
        private OVRInput.Controller ovrController = OVRInput.Controller.None;

        public bool ButtonStartDown => OVRInput.GetDown(OVRInput.Button.Start, ovrController);

        public bool ButtonOneDown => OVRInput.GetDown(OVRInput.Button.One, ovrController);

        public bool ButtonTwoDown => OVRInput.GetDown(OVRInput.Button.Two, ovrController);

        public bool TriggerPrimaryDown => OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, ovrController);

        public bool TriggerPrimaryUp => OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, ovrController);

        public bool TriggerSecondaryDown => OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, ovrController);

        public bool TriggerSecondaryUp => OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, ovrController);

        public bool TriggerInteractionDown
        {
            get
            {
                if(PlayerSettingsController.Instance.InteractionButtonMode == InteractionButtonModeEnum.Grip)
                {
                    return TriggerSecondaryDown;
                }
                else
                {
                    return TriggerPrimaryDown;
                }
            }
        }

        public bool TriggerInteractionUp
        {
            get
            {
                if (PlayerSettingsController.Instance.InteractionButtonMode == InteractionButtonModeEnum.Grip)
                {
                    return TriggerSecondaryUp;
                }
                else
                {
                    return TriggerPrimaryUp;
                }
            }
        }

        public float TriggerPrimaryPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, ovrController);

        public float TriggerSecondaryPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, ovrController);

        public float TriggerInteractionPressure
        {
            get
            {
                if (PlayerSettingsController.Instance.InteractionButtonMode == InteractionButtonModeEnum.Grip)
                {
                    return TriggerPrimaryPressure;
                }
                else
                {
                    return TriggerSecondaryPressure;
                }
            }
        }

        public float ThumbPressure
        {
            get
            {
                if (OVRInput.Get(OVRInput.Touch.One, ovrController) || OVRInput.Get(OVRInput.Touch.Two, ovrController) ||
                    OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, ovrController) || OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, ovrController))
                {
                    return 1f;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public float IndexFingerPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, ovrController);

        public float MiddleFingerPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, ovrController);

        public float RingFingerPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, ovrController);

        public float PinkyPressure => OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, ovrController);

        public Vector2 ThumbstickPrimary
        {
            get
            {
                var thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ovrController);

                if(thumbstickInput.x > -THUMBSTICK_DEADZONE && thumbstickInput.x < THUMBSTICK_DEADZONE &&
                    thumbstickInput.y > -THUMBSTICK_DEADZONE && thumbstickInput.y < THUMBSTICK_DEADZONE)
                {
                    return Vector2.zero;
                }
                else
                {
                    return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, ovrController);
                }
            }
        }
    }
}
