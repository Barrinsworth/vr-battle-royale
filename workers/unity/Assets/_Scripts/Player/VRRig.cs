using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public abstract class VRRig : MonoBehaviour
    {
        [SerializeField] protected Transform hmdTransform;
        [SerializeField] protected Transform leftHandAnchorTransform;
        [SerializeField] protected Transform rightHandAnchorTransform;
        [SerializeField] protected FOVBlinders fovBlinders;
        [SerializeField] private float smoothRotationMultiplier = 30;
        [SerializeField] private float snapRotationCooldown = 0.3f;

        private float lastSnapRotationTime = 0f;

        public Transform HMDTransform { get { return hmdTransform; } }
        public Transform LeftHandAnchorTransform { get { return leftHandAnchorTransform; } }
        public Transform RightHandAnchorTransform { get { return rightHandAnchorTransform; } }
        public Transform MoveHand
        {
            get
            {
                if(PlayerSettingsManager.Instance.MoveHand == HandednessEnum.Left)
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

        #region Unity Life Cycle
        private void Update()
        {
            HandleRotationInput();
        }
        #endregion

        private void HandleRotationInput()
        {
            var rotationInput = RotationInput;

            if (rotationInput == 0f)
            {
                return;
            }

            var deltaRotation = 0f;

            if (PlayerSettingsManager.Instance.RotationMode == RotationModeEnum.Smooth)
            {
                deltaRotation = rotationInput * PlayerSettingsManager.Instance.SmoothRotationSpeed * smoothRotationMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                if (Time.time - lastSnapRotationTime >= snapRotationCooldown)
                {
                    lastSnapRotationTime = Time.time;

                    deltaRotation = rotationInput > 0 ? PlayerSettingsManager.Instance.SnapRotationDegrees : -PlayerSettingsManager.Instance.SnapRotationDegrees;
                }
            }

            if (deltaRotation != 0f)
            {
                var yEulerAngle = 0f;

                if (PlayerSettingsManager.Instance.RoomSetup == RoomSetupEnum.Roomscale)
                {
                    yEulerAngle = Camera.main.transform.eulerAngles.y + deltaRotation;
                }
                else
                {
                    yEulerAngle = PlayerManager.Instance.CurrentVRRig.transform.eulerAngles.y + deltaRotation;
                }

                TeleportPlayerHead(Camera.main.transform.position, yEulerAngle);
            }
        }

        #region Teleports
        private void TeleportPlayerRoom(Vector3 desiredWorldPositionOfRoom, Quaternion desiredWordRotationOfRoom)
        {
            transform.rotation = desiredWordRotationOfRoom;
            transform.position = desiredWorldPositionOfRoom;
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera)
        {
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (transform.position - Camera.main.transform.position), transform.rotation);
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera, float lookAtYEulerAngle)
        {
            if (PlayerSettingsManager.Instance.RoomSetup == RoomSetupEnum.Roomscale)
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.eulerAngles.y, 0f) *
                    (transform.position - Camera.main.transform.position)), Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.localEulerAngles.y, 0f));
            }
            else
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - transform.eulerAngles.y, 0f) *
                    (transform.position - Camera.main.transform.position)), Quaternion.Euler(0f, lookAtYEulerAngle, 0f));
            }
        }
        #endregion
    }
}
