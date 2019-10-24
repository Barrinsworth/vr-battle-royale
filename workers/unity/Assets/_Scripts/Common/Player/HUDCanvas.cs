using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRBattleRoyale.Common;

namespace VRBattleRoyale.Common.Player
{
    public class HUDCanvas : MonoBehaviour
    {
        [Header("--HUD Canvas--")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera oculusCamera;
        [SerializeField] private Camera openVRCamera;
        [SerializeField] private Camera playstationVRCamera;

        private void Awake()
        {
            switch (SessionController.Instance.CurrentHMDType)
            {
                case HMDTypeEnum.OculusQuest:
                case HMDTypeEnum.OculusRift:
                    canvas.worldCamera = oculusCamera;
                    break;
                case HMDTypeEnum.OpenVR:
                    canvas.worldCamera = openVRCamera;
                    break;
                case HMDTypeEnum.PlayStationVR:
                    canvas.worldCamera = playstationVRCamera;
                    break;
                default:
                    canvas.worldCamera = openVRCamera;
                    break;
            }

            canvas.planeDistance = canvas.worldCamera.nearClipPlane * 1.1f;
        }
    }
}
