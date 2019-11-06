using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;

namespace VRBattleRoyale.Singleplayer
{
    public class PlayerController_Singleplayer : MonoBehaviour
    {
        private static PlayerController_Singleplayer instance;

        public static PlayerController_Singleplayer Instance { get { return instance; } }

        [Header("--Player Controller Single Player--")]
        [SerializeField] private VRRig oculusRig;
        [SerializeField] private VRRig openVRRig;
        [SerializeField] private VRRig playstationVRRig;
        [SerializeField] private PlayerMotor_Singleplayer playerMotor;

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
