using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Vitruvius.Generated.Player;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient)]
    public class PlayerController_AuthoritativeClient : MonoBehaviour
    {
        private static PlayerController_AuthoritativeClient instance;

        public static PlayerController_AuthoritativeClient Instance { get { return instance; } }

        [Require] private PlayerAchorsWriter anchorsWriter;
        [Require] private EntityId entityId;

        [SerializeField] private Transform hmd;
        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;

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

                anchorsUpdate.HmdPosition = Vector3Util.ConvertToSpatialOSVector3(hmd.localPosition);
                anchorsUpdate.HmdRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(hmd.localRotation);

                anchorsUpdate.RightHandPosition = Vector3Util.ConvertToSpatialOSVector3(rightHand.localPosition);
                anchorsUpdate.RightHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(rightHand.localRotation);

                anchorsUpdate.LeftHandPosition = Vector3Util.ConvertToSpatialOSVector3(leftHand.localPosition);
                anchorsUpdate.LeftHandRotation = QuaternionUtil.ConvertToSpatialOSQuaternion(leftHand.localRotation);

                anchorsWriter.SendUpdate(anchorsUpdate);

                yield return new WaitForSecondsRealtime(0.03333f);
            }

            updateAnchorsCoroutine = null;
        }
    }
}
