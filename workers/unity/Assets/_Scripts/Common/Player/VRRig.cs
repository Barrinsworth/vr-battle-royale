using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public abstract class VRRig : MonoBehaviour
    {
        [Header("--VR Rig--")]
        [SerializeField] protected Transform hmdTransform;
        [SerializeField] protected Transform leftHandAnchorTransform;
        [SerializeField] protected Transform rightHandAnchorTransform;
        [SerializeField] protected FOVBlinders fovBlinders;

        public Transform MoveHand
        {
            get
            {
                if(PlayerSettingsController.Instance.MoveHand == HandednessEnum.Left)
                {
                    return leftHandAnchorTransform;
                }
                else
                {
                    return rightHandAnchorTransform;
                }
            }
        }
        public FOVBlinders FOVBlinders { get { return fovBlinders; } }
        public abstract IControllerInput LeftHandControllerInput { get; }
        public abstract IControllerInput RightHandControllerInput { get; }
        public abstract Vector2 MovementInput { get; }
        public abstract float RotationInput { get; }
        public abstract bool JumpButtonPressed { get; }
        public abstract bool CrouchButtonPressed { get; }
    }
}
