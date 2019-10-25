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
    public class PlayerController_AuthoritativeClient : MonoBehaviour
    {
        private static PlayerController_AuthoritativeClient instance;

        public static PlayerController_AuthoritativeClient Instance { get { return instance; } }

        [Require] private PlayerAchorsWriter anchorsWriter;
        [Require] private EntityId entityId;

        [Header("--Player Controller Authoritative Client--")]
        [SerializeField] private VRRig oculusRig;
        [SerializeField] private VRRig openVRRig;
        [SerializeField] private VRRig playstationVRRig;

        private VRRig currentVRRig;

        public VRRig CurrentVRRig { get { return currentVRRig; } }

        private Coroutine updateAnchorsCoroutine;

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

        private void OnEnable()
        {
            if(anchorsWriter != null)
            {
                updateAnchorsCoroutine = StartCoroutine(UpdareAnchorsCoroutine());
            }
        }

        private void OnDisable()
        {
            if(updateAnchorsCoroutine != null)
            {
                StopCoroutine(updateAnchorsCoroutine);
                updateAnchorsCoroutine = null;
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

        private IEnumerator UpdareAnchorsCoroutine()
        {
            while(true)
            {
                var anchorsUpdate = new PlayerAchors.Update();

                anchorsUpdate.HmdPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.HMDTransform.localPosition);
                anchorsUpdate.HmdRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.HMDTransform.localRotation);

                anchorsUpdate.RightHandPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.RightHandAnchorTransform.localPosition);
                anchorsUpdate.RightHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.RightHandAnchorTransform.localRotation);

                anchorsUpdate.LeftHandPosition = Vector3Util.ConvertToSpatialOSVector3(CurrentVRRig.LeftHandAnchorTransform.localPosition);
                anchorsUpdate.LeftHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(CurrentVRRig.LeftHandAnchorTransform.localRotation);

                anchorsWriter.SendUpdate(anchorsUpdate);

                yield return new WaitForSecondsRealtime(0.03333f);
            }

            updateAnchorsCoroutine = null;
        }
    }
}
