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
        public SettingsChangedEvent OnMovementOrientationModeChanged;
        public SettingsChangedEvent OnRoomSetupChanged;
        public SettingsChangedEvent OnFOVBlindersEnabledChanged;
        public SettingsChangedEvent OnFOVBlindersStrengthChanged;

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
                var found = false;

                for(var i = 0; i < PlayerSettings.SNAP_DEGREES.Length; i++)
                {
                    if(value == PlayerSettings.SNAP_DEGREES[i])
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
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
                playerSettings.SmoothRotationSpeed = Mathf.Clamp(value, PlayerSettings.MIN_SMOOTH_ROTATION, PlayerSettings.MAX_SMOOTH_ROTATION);

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
        public bool FOVBlindersEnabled
        {
            get { return playerSettings.fovBlindersEnabled; }
            set
            {
                playerSettings.fovBlindersEnabled = value;

                if(OnFOVBlindersEnabledChanged != null)
                {
                    OnFOVBlindersEnabledChanged();
                }
            }
        }
        public int FOVBlindersStrength
        {
            get { return playerSettings.fovBlindersStrength; }
            set
            {
                playerSettings.fovBlindersStrength = Mathf.Clamp(value, PlayerSettings.MIN_FOV_BLINDERS_STRENGTH, PlayerSettings.MAX_FOV_BLINDERS_STRENGTH);

                if (OnFOVBlindersStrengthChanged != null)
                {
                    OnFOVBlindersStrengthChanged();
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

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;

                OnMoveHandChanged = null;
                OnRotationModeChanged = null;
                OnSnapRotationDegreesChanged = null;
                OnSmoothRotationSpeedChanged = null;
                OnInteractionButtonModeChanged = null;
                OnMovementOrientationModeChanged = null;
                OnRoomSetupChanged = null;
                OnFOVBlindersEnabledChanged = null;
                OnFOVBlindersStrengthChanged = null;
            }
        }
        #endregion
    }
}
