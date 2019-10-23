using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public class PlayerSettingsController : MonoBehaviour
    {
        private static double MIN_SMOOTH_ROTATION = 0.5;
        private static double MAX_SMOOTH_ROTATION = 10;

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
        public SettingsChangedEvent OnMovementOrientationModeChanged;
        public SettingsChangedEvent OnRoomSetupChanged;

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
        public double SmoothRotationSpeed
        {
            get { return playerSettings.SmoothRotationSpeed; }
            set
            {
                if (value < MIN_SMOOTH_ROTATION || value > MAX_SMOOTH_ROTATION)
                {
                    return;
                }

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
        public MovementOrientationModeEnum MovementOrientationMode
        {
            get { return playerSettings.MovementOrientationMode; }
            set
            {
                playerSettings.MovementOrientationMode = value;

                if(OnMovementOrientationModeChanged != null)
                {
                    OnMovementOrientationModeChanged();
                }
            }
        }
        public RoomSetupEnum RoomSetup
        {
            get { return playerSettings.RoomSetup; }
            set
            {
                playerSettings.RoomSetup = value;

                if(OnRoomSetupChanged != null)
                {
                    OnRoomSetupChanged();
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
