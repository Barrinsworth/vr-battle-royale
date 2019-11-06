using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    [CreateAssetMenu(fileName = "New Player Settings", menuName = "ScriptableObjects/Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public static float MIN_SMOOTH_ROTATION = 0.5f;
        public static float MAX_SMOOTH_ROTATION = 10f;
        public static int[] SNAP_DEGREES = { 15, 30, 45, 60, 90 };
        public static int MIN_FOV_BLINDERS_STRENGTH = 1;
        public static int MAX_FOV_BLINDERS_STRENGTH = 10;

        public HandednessEnum MoveHand = HandednessEnum.Left;
        public RotationModeEnum RotationMode = RotationModeEnum.Snap;
        public int SnapRotationDegrees = 30;
        public float SmoothRotationSpeed = 5;
        public InteractionButtonModeEnum InteractionButtonMode = InteractionButtonModeEnum.Grip;
        public MovementOrientationModeEnum MovementOrientationMode = MovementOrientationModeEnum.Hand;
        public RoomSetupEnum RoomSetup = RoomSetupEnum.Roomscale;
        public bool fovBlindersEnabled = true;
        public int fovBlindersStrength = 5;
    }
}
