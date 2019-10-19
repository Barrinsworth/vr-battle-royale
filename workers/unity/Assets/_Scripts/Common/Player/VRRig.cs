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

        public abstract IControllerInput LeftHandControllerInput { get; }

        public abstract IControllerInput RightHandControllerInput { get; }
        public abstract Vector2 MovementInput { get; }
        public abstract float RotationInput { get; }
    }
}
