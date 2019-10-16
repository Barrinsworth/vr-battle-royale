using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Player;

namespace VRBattleRoyale
{
    //[WorkerType(WorkerUtils.UnityClient)]
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

                anchorsUpdate.HmdPosition = ConvertToPlayerVector3(hmd.localPosition);
                anchorsUpdate.HmdRotation = ConvertToPlayerQuaternion(hmd.localRotation);

                anchorsUpdate.RightHandPosition = ConvertToPlayerVector3(rightHand.localPosition);
                anchorsUpdate.RightHandRotation = ConvertToPlayerQuaternion(rightHand.localRotation);

                anchorsUpdate.LeftHandPosition = ConvertToPlayerVector3(leftHand.localPosition);
                anchorsUpdate.LeftHandRotation = ConvertToPlayerQuaternion(leftHand.localRotation);

                anchorsWriter.SendUpdate(anchorsUpdate);

                yield return new WaitForSecondsRealtime(0.03333f);
            }

            updateAnchorsCoroutine = null;
        }

        private Player.Vector3 ConvertToPlayerVector3(UnityEngine.Vector3 vector3)
        {
            return new Player.Vector3(vector3.x, vector3.y, vector3.z);
        }

        private Player.Quaternion ConvertToPlayerQuaternion(UnityEngine.Quaternion quaternion)
        {
            return new Player.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}
