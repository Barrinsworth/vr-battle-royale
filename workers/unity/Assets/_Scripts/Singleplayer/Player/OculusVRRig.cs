﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;

namespace VRBattleRoyale.SinglePlayer
{
    public class OculusVRRig : VRRig
    {
        [Header("--Oculus VR Rig--")]
        [SerializeField] protected OculusControllerInput leftHandControllerInput;
        [SerializeField] protected OculusControllerInput rightHandControllerInput;

        public override IControllerInput LeftHandControllerInput { get { return leftHandControllerInput; } }

        public override IControllerInput RightHandControllerInput { get { return rightHandControllerInput; } }

        public override Vector2 MovementInput
        {
            get
            {
                if(PlayerSettingsController.Instance.MoveHand == HandednessEnum.Left)
                {
                    return LeftHandControllerInput.ThumbstickPrimary;
                }
                else
                {
                    return RightHandControllerInput.ThumbstickPrimary;
                }
            }
        }

        public override float RotationInput
        {
            get
            {
                if (PlayerSettingsController.Instance.MoveHand == HandednessEnum.Left)
                {
                    return RightHandControllerInput.ThumbstickPrimary.x;
                }
                else
                {
                    return LeftHandControllerInput.ThumbstickPrimary.x;
                }
            }
        }
    }
}
