using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Vitruvius.Generated.Player;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient, WorkerUtils.UnityGameLogic)]
    public class PlayerController_Multiplayer_Proxy : MonoBehaviour
    {
        [Require] private PlayerAchorsReader anchorsReader;
        [Require] private EntityId entityId;

        [Header("--Player Controller Multiplayer Proxy--")]
        [SerializeField] private Transform hmd;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;

        public Transform HMD { get { return hmd; } }

        #region Unity Life Cycle
        private void OnEnable()
        {
            anchorsReader.OnUpdate += AnchorsUpdated;
        }

        private void OnDisable()
        {
            anchorsReader.OnUpdate -= AnchorsUpdated;
        }
        #endregion

        #region SpatialOS Event Listeners
        private void AnchorsUpdated(PlayerAchors.Update update)
        {
            hmd.localPosition = Vector3Util.ConvertToUnityVector3(update.HmdPosition);
            hmd.localRotation = QuaternionUtil.ConvertToUnityQuaternion(update.HmdRotation);

            rightHand.localPosition = Vector3Util.ConvertToUnityVector3(update.RightHandPosition);
            rightHand.localRotation = QuaternionUtil.ConvertToUnityQuaternion(update.RightHandRotation);

            leftHand.localPosition = Vector3Util.ConvertToUnityVector3(update.LeftHandPosition);
            leftHand.localRotation = QuaternionUtil.ConvertToUnityQuaternion(update.LeftHandRotation);
        }
        #endregion
    }
}
