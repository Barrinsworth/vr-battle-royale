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
        [SerializeField] private PlayerMotor playerMotor;
        [SerializeField] private FOVBlinders fovBlinders;

        private VRRig currentVRRig;

        public VRRig CurrentVRRig { get { return currentVRRig; } }
        public FOVBlinders FOVBlinders { get { return fovBlinders; } }

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
