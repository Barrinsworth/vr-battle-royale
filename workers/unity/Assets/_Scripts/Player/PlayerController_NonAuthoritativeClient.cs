using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Player;

namespace VRBattleRoyale
{
    //[WorkerType(WorkerUtils.UnityClient)]
    public class PlayerController_NonAuthoritativeClient : MonoBehaviour
    {
        [Require] private PlayerAchorsReader anchorsReader;
        [Require] private EntityId entityId;

        [SerializeField] private Transform hmd;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;

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
            hmd.localPosition = ConvertToUnityVector3(update.HmdPosition);
            hmd.localRotation = ConvertToUnityQuaternion(update.HmdRotation);

            rightHand.localPosition = ConvertToUnityVector3(update.RightHandPosition);
            rightHand.localRotation = ConvertToUnityQuaternion(update.RightHandRotation);

            leftHand.localPosition = ConvertToUnityVector3(update.LeftHandPosition);
            leftHand.localRotation = ConvertToUnityQuaternion(update.LeftHandRotation);
        }
        #endregion

        private UnityEngine.Vector3 ConvertToUnityVector3(Player.Vector3 vector3)
        {
            return new UnityEngine.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        private UnityEngine.Quaternion ConvertToUnityQuaternion(Player.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
