using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VRBattleRoyale
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        public static InputManager Instance { get { return instance; } }

        private Input input;
        private InputAction moveAction;
        private InputAction rotateAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction moveInputOrientationAction;

        public Vector3 MoveInput
        {
            get
            {
                var movementInput = moveAction.ReadValue<Vector2>();
                var orientationYEulerAngle = moveInputOrientationAction.ReadValue<Quaternion>().eulerAngles.y;
                var rotatedMoveInput = Quaternion.Euler(0f, orientationYEulerAngle, 0f) * new Vector3(movementInput.x, 0f, movementInput.y);

                if(rotatedMoveInput.magnitude > 1f)
                {
                    rotatedMoveInput.Normalize();
                }

                return rotatedMoveInput;
            }
        }
        public float RotationInput { get { return rotateAction.ReadValue<float>(); } }
        public bool JumpInput { get { return jumpAction.triggered; } }
        public bool CrouchInput { get { return crouchAction.triggered; } }

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

            input = new Input();
        }

        private void OnEnable()
        {
            input.Enable();

            if(PlayerSettingsManager.Instance)
            {
                PlayerSettingsManager.Instance.OnInputLayoutChanged += InputLayoutChanged;
                PlayerSettingsManager.Instance.OnMovementOrientationModeChanged += MovementOrientationModeChanged;
            }
        }

        private void Start()
        {
            InitializeInputActions();
        }

        private void OnDisable()
        {
            input.Disable();

            if (PlayerSettingsManager.Instance)
            {
                PlayerSettingsManager.Instance.OnInputLayoutChanged -= InputLayoutChanged;
                PlayerSettingsManager.Instance.OnMovementOrientationModeChanged -= MovementOrientationModeChanged;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                input.Disable();
                input.Dispose();

                instance = null;
            }
        }
        #endregion

        #region Event Listeners
        private void InputLayoutChanged()
        {
            InitializeInputActions();
        }

        private void MovementOrientationModeChanged()
        {
            InitializeInputActions();
        }
        #endregion

        private void InitializeInputActions()
        {
            if(PlayerSettingsManager.Instance == null || PlayerSettingsManager.Instance.InputLayout == InputLayoutEnum.Default)
            {
                moveAction = input.Gameplay.LeftMove;
                rotateAction = input.Gameplay.RightRotate;
                jumpAction = input.Gameplay.RightJump;
                crouchAction = input.Gameplay.RightCrouch;

                if (PlayerSettingsManager.Instance == null || PlayerSettingsManager.Instance.MovementOrientationMode == MovementOrientationModeEnum.Head)
                {
                    moveInputOrientationAction = input.Gameplay.HMDRotation;
                }
                else
                {
                    moveInputOrientationAction = input.Gameplay.LeftHandRotation;
                }
            }
            else
            {
                moveAction = input.Gameplay.RightMove;
                rotateAction = input.Gameplay.LeftRotate;
                jumpAction = input.Gameplay.LeftJump;
                crouchAction = input.Gameplay.LeftCrouch;

                if (PlayerSettingsManager.Instance == null || PlayerSettingsManager.Instance.MovementOrientationMode == MovementOrientationModeEnum.Head)
                {
                    moveInputOrientationAction = input.Gameplay.HMDRotation;
                }
                else
                {
                    moveInputOrientationAction = input.Gameplay.RightHandRotation;
                }
            }
        }
    }
}
