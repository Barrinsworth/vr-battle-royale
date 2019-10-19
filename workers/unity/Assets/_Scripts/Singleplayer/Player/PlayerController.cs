using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;

namespace VRBattleRoyale.SinglePlayer
{
    public class PlayerController : MonoBehaviour
    {
        private static PlayerController instance;

        public static PlayerController Instance { get { return instance; } }

        [Header("--Player Controller--")]
        [SerializeField] private VRRig oculusRig;
        [SerializeField] private VRRig openVRRig;
        [SerializeField] private VRRig playstationVRRig;

        public VRRig CurrentVRRig
        {
            get
            {
                switch (SessionController.Instance.CurrentHMDType)
                {
                    case HMDTypeEnum.OculusQuest:
                    case HMDTypeEnum.OculusRift:
                        return oculusRig;
                    case HMDTypeEnum.OpenVR:
                        return openVRRig;
                    case HMDTypeEnum.PlayStationVR:
                        return playstationVRRig;
                    default:
                        return openVRRig;
                }
            }
        }

        #region Unity Life Cycle
        private void Awake()
        {
            if(instance != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(gameObject);
#else
                Destroy(gameObject);
#endif
                return;
            }

            instance = this;

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
    }
}
