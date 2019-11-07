using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public class PlayerSettingsManager : MonoBehaviour
    {
        private static PlayerSettingsManager instance;

        public static PlayerSettingsManager Instance { get { return instance; } }

        [SerializeField] private PlayerSettings playerSettings;

        public delegate void SettingsChangedEvent();
        public SettingsChangedEvent OnDominantHandChanged;
        public SettingsChangedEvent OnInputLayoutChanged;
        public SettingsChangedEvent OnRotationModeChanged;
        public SettingsChangedEvent OnSnapRotationDegreesChanged;
        public SettingsChangedEvent OnSmoothRotationSpeedChanged;
        public SettingsChangedEvent OnMovementOrientationModeChanged;
        public SettingsChangedEvent OnRoomSetupChanged;
        public SettingsChangedEvent OnFOVBlindersEnabledChanged;
        public SettingsChangedEvent OnFOVBlindersStrengthChanged;

        public HandednessEnum DominantHand
        {
            get { return playerSettings.DominantHand; }
            set
            {
                playerSettings.DominantHand = value;

                if(OnDominantHandChanged != null)
                {
                    OnDominantHandChanged();
                }
            }
        }
        public InputLayoutEnum InputLayout
        {
            get { return playerSettings.InputLayout; }
            set
            {
                playerSettings.InputLayout = value;

                if (OnInputLayoutChanged != null)
                {
                    OnInputLayoutChanged();
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

            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;

                OnDominantHandChanged = null;
                OnInputLayoutChanged = null;
                OnRotationModeChanged = null;
                OnSnapRotationDegreesChanged = null;
                OnSmoothRotationSpeedChanged = null;
                OnMovementOrientationModeChanged = null;
                OnRoomSetupChanged = null;
                OnFOVBlindersEnabledChanged = null;
                OnFOVBlindersStrengthChanged = null;
            }
        }
        #endregion
    }
}
