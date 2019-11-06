using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace VRBattleRoyale
{
    public class PlayerManager : MonoBehaviour, IReceiveEntity
    {
        private static PlayerManager instance;

        public static PlayerManager Instance { get { return instance; } }

        [SerializeField] private Transform rigTransform;
        [SerializeField] private Transform hmdTransform;
        [SerializeField] private Transform leftHandAnchorTransform;
        [SerializeField] private Transform rightHandAnchorTransform;
        [SerializeField] private FOVBlinders fovBlinders;
        [SerializeField] private float smoothRotationMultiplier = 30;
        [SerializeField] private float snapRotationCooldown = 0.3f;

        private float lastSnapRotationTime = 0f;
        private Entity characterControllerEntity = Entity.Null;

        public Transform RigTransform { get { return rigTransform; } }
        public Transform HMDTransform { get { return hmdTransform; } }
        public Transform LeftHandAnchorTransform { get { return leftHandAnchorTransform; } }
        public Transform RightHandAnchorTransform { get { return rightHandAnchorTransform; } }
        public Transform MoveHand
        {
            get
            {
                if (PlayerSettingsManager.Instance.DominantHand == HandednessEnum.Left)
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
        public Vector2 MovementInput { get { return Vector2.zero; } }
        public float RotationInput { get { return 0f; } }
        public bool JumpButtonPressed { get { return false; } }
        public bool CrouchButtonPressed { get { return false; } }

        #region Unity Life Cycle
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void OnEnable()
        {
            UnityEngine.XR.XRDevice.deviceLoaded += VRDeviceLoaded;   
        }

        private void Start()
        {
            if (UnityEngine.XR.XRDevice.isPresent)
                VRDeviceLoaded(UnityEngine.XR.XRDevice.model);
        }

        private void Update()
        {
            HandleRotationInput();
        }

        private void OnDisable()
        {
            UnityEngine.XR.XRDevice.deviceLoaded -= VRDeviceLoaded;
        }

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
        #endregion

        #region Event Listeners
        private void VRDeviceLoaded(string device)
        {
            Debug.Log("LOADED");
            Camera.main.fieldOfView = math.radians(UnityEngine.XR.XRSettings.eyeTextureWidth / UnityEngine.XR.XRSettings.eyeTextureHeight) * 2.0f;
        }

        public void SetReceivedEntity(Entity entity)
        {
            characterControllerEntity = entity;
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
                    yEulerAngle = RigTransform.eulerAngles.y + deltaRotation;
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
