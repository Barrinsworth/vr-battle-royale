using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public class PlayerSettingsController : MonoBehaviour
    {
        private static PlayerSettingsController instance;

        public static PlayerSettingsController Instance { get { return instance; } }

        [Header("--Player Settings Controller--")]
        [SerializeField] private PlayerSettings playerSettings;

        public delegate void SettingsChangedEvent();
        public SettingsChangedEvent OnMoveHandChanged;
        public SettingsChangedEvent OnRotationModeChanged;
        public SettingsChangedEvent OnSnapRotationDegreesChanged;
        public SettingsChangedEvent OnSmoothRotationSpeedChanged;
        public SettingsChangedEvent OnInteractionButtonModeChanged;

        public HandednessEnum MoveHand
        {
            get { return playerSettings.MoveHand; }
            set
            {
                playerSettings.MoveHand = value;

                if(OnMoveHandChanged != null)
                {
                    OnMoveHandChanged();
                }
            }
        }
        public RotationModeEnum RotationMode
        {
            get { return playerSettings.RotationMode; }
            set
            {
                playerSettings.RotationMode = value;

                if (OnRotationModeChanged != null)
                {
                    OnRotationModeChanged();
                }
            }
        }
        public int SnapRotationDegrees
        {
            get { return playerSettings.SnapRotationDegrees; }
            set
            {
                if(value != 15 && value != 30 && value != 45 && value != 60 && value != 90)
                {
                    return;
                }

                playerSettings.SnapRotationDegrees = value;

                if (OnSnapRotationDegreesChanged != null)
                {
                    OnSnapRotationDegreesChanged();
                }
            }
        }
        public float SmoothRotationSpeed
        {
            get { return playerSettings.SmoothRotationSpeed; }
            set
            {
                if (value < 1 || value > 10)
                {
                    return;
                }

                playerSettings.SmoothRotationSpeed = value;

                if (OnSmoothRotationSpeedChanged != null)
                {
                    OnSmoothRotationSpeedChanged();
                }
            }
        }
        public InteractionButtonModeEnum InteractionButtonMode
        {
            get { return playerSettings.InteractionButtonMode; }
            set
            {
                playerSettings.InteractionButtonMode = value;

                if (OnInteractionButtonModeChanged != null)
                {
                    OnInteractionButtonModeChanged();
                }
            }
        }

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
        }

        private void OnDisable()
        {
            OnMoveHandChanged = null;
            OnRotationModeChanged = null;
            OnSnapRotationDegreesChanged = null;
            OnSmoothRotationSpeedChanged = null;
            OnInteractionButtonModeChanged = null;
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
