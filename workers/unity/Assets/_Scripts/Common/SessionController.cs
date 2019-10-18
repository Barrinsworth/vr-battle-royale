using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common
{
    public class SessionController : MonoBehaviour
    {
        private static SessionController instance;

        public static SessionController Instance { get { return instance; } }

        [Header("--Session Controller--")]
        [SerializeField] private SessionSettings sessionSettings;

        public HMDTypeEnum CurrentHMDType { get { return sessionSettings.HMDType; } }

        #region Unity Life Cycle
        private void Awake()
        {
            if (instance != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(gameObject);
#else
                Destroy(gameObject);
#endif
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

#if !UNITY_EDITOR
            Debug.Log("TODO - REFINE THIS TO CHECK DEVICE NAME AND IF ITS LOADED?");
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Equals("oculus"))
                    sessionSettings.HMDType = HMDTypeEnum.OculusRift;
                else if (UnityEngine.XR.XRSettings.loadedDeviceName.ToLower().Equals("openvr"))
                    sessionSettings.HMDType = HMDTypeEnum.OpenVR;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                sessionSettings.HMDType = HMDTypeEnum.OculusQuest;
            }
            else if (Application.platform == RuntimePlatform.PS4)
            {
                sessionSettings.HMDType = HMDTypeEnum.PlayStationVR;
            }
#endif
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        #endregion
    }
}
