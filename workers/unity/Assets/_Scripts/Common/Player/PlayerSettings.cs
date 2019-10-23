using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    [CreateAssetMenu(fileName = "New Player Settings", menuName = "ScriptableObjects/Common/Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public HandednessEnum MoveHand = HandednessEnum.Left;
        public RotationModeEnum RotationMode = RotationModeEnum.Snap;
        public int SnapRotationDegrees = 30;
        public double SmoothRotationSpeed = 5;
        public InteractionButtonModeEnum InteractionButtonMode = InteractionButtonModeEnum.Grip;
        public MovementOrientationModeEnum MovementOrientationMode = MovementOrientationModeEnum.Hand;
        public RoomSetupEnum RoomSetup = RoomSetupEnum.Roomscale;
    }
}
