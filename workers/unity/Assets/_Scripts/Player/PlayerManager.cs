using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace VRBattleRoyale
{
    public class PlayerManager : MonoBehaviour, IReceiveEntity
    {
        private static PlayerManager instance;

        public static PlayerManager Instance { get { return instance; } }

        [SerializeField] private VRRig oculusRig;
        [SerializeField] private VRRig openVRRig;
        [SerializeField] private VRRig playstationVRRig;

        private VRRig currentVRRig;
        private Entity characterControllerEntity = Entity.Null;

        public VRRig CurrentVRRig { get { return currentVRRig; } }

        #region Unity Life Cycle
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            switch (SessionSettingsManager.Instance.CurrentHMDType)
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

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
        #endregion

        public void SetReceivedEntity(Entity entity)
        {
            characterControllerEntity = entity;
        }
    }
}
