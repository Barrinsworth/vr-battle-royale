using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Vitruvius.Generated.Player;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient)]
    public class PlayerController_Multiplayer_Client : MonoBehaviour
    {
        private static PlayerController_Multiplayer_Client instance;

        public static PlayerController_Multiplayer_Client Instance { get { return instance; } }

        [Require] private PlayerAchorsWriter anchorsWriter;
        [Require] private EntityId entityId;

        [Header("--Player Controller Multiplayer Client--")]
        [SerializeField] private VRRig oculusRig;
        [SerializeField] private VRRig openVRRig;
        [SerializeField] private VRRig playstationVRRig;
        [SerializeField] private PlayerMotor_Multiplayer_Client playerMotor;
        [SerializeField] private float anchorsUpdateFrequency = 0.03333f;

        private float timeSinceLastAnchorUpdate = 0f;
        private VRRig currentVRRig;

        public VRRig CurrentVRRig { get { return currentVRRig; } }
        public Vector3 Position { get { return CurrentVRRig.transform.position; } set { CurrentVRRig.transform.position = value; } }
        public Quaternion Rotation { get { return CurrentVRRig.transform.rotation; } set { CurrentVRRig.transform.rotation = value; } }
        public float YEulerAngle { get { return CurrentVRRig.transform.eulerAngles.y; } }

        #region Unity Life Cycle
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            switch (SessionController.Instance.CurrentHMDType)
            {
                case HMDTypeEnum.OculusQuest:
                case HMDTypeEnum.OculusRift:
                    currentVRRig = oculusRig;
                    break;
                case HMDTypeEnum.OpenVR:
                    currentVRRig = openVRRig;
                    break;
                case HMDTypeEnum.PlayStationVR:
                    currentVRRig = playstationVRRig;
                    break;
                default:
                    currentVRRig = openVRRig;
                    break;
            }

            CurrentVRRig.gameObject.SetActive(true);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(CurrentVRRig.LeftHandControllerInput.TriggerPrimaryDown)
            {
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif

            timeSinceLastAnchorUpdate += Time.deltaTime;

            if(timeSinceLastAnchorUpdate >= anchorsUpdateFrequency)
            {
                var anchorsUpdate = new PlayerAchors.Update();

                anchorsUpdate.HmdPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.HMDTransform.localPosition);
                anchorsUpdate.HmdRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.HMDTransform.localRotation);

                anchorsUpdate.RightHandPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.RightHandAnchorTransform.localPosition);
                anchorsUpdate.RightHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.RightHandAnchorTransform.localRotation);

                anchorsUpdate.LeftHandPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.LeftHandAnchorTransform.localPosition);
                anchorsUpdate.LeftHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.LeftHandAnchorTransform.localRotation);

                anchorsWriter.SendUpdate(anchorsUpdate);

                timeSinceLastAnchorUpdate = 0f;
            }
        }

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
        #endregion
    }
}
