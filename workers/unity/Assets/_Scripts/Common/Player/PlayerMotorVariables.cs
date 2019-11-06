using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    [CreateAssetMenu(fileName = "New Player Motor Variables", menuName = "ScriptableObjects/Common/Player Motor Variables")]
    public class PlayerMotorVariables : ScriptableObject
    {
        public float MovementSpeed = 7f;
        public float JumpSpeed = 7f;
        public float JumpDuration = 0.2f;
        public float ExtraJumpTimeAfterLeavingGround = 0.15f;
        public float StepHeightWorldUnits = 0.4f;
        public int SlopeLimit = 70;
        [Range(0f, 1f)] public float AirControl = 0.4f;
        public float Gravity = 30f;
        public float SlideGravity = 30f;
        public float AirFriction = 0.5f;
        public float GroundFriction = 100f;
        public float SmoothRotationMultiplier = 30;
        public float SnapRotationCooldown = 0.3f;
        public float CrouchDistance = 0.5f;
        public float CameraForwardOffset = -0.05f;
        public float CameraSmoothTime = 0.05f;
    }
}
