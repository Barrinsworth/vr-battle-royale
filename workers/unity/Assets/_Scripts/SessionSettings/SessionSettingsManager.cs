using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public class SessionSettingsManager : MonoBehaviour
    {
        private static SessionSettingsManager instance;

        public static SessionSettingsManager Instance { get { return instance; } }

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

            if(sessionSettings.HMDType == HMDTypeEnum.OculusQuest)
            {
                Time.fixedDeltaTime = 1 / 72f;
            }
            else if (sessionSettings.HMDType == HMDTypeEnum.OculusRift)
            {
                Time.fixedDeltaTime = 1 / 90f;
            }
            else if (sessionSettings.HMDType == HMDTypeEnum.OpenVR)
            {
                Time.fixedDeltaTime = 1 / 90f;
            }
            else if (sessionSettings.HMDType == HMDTypeEnum.PlayStationVR)
            {
                Time.fixedDeltaTime = 1 / 60f;
            }
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
