// GENERATED AUTOMATICALLY FROM 'Assets/Input/Input.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace VRBattleRoyale
{
    public class @Input : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public @Input()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""49bce3f6-c492-48a4-9de6-1ae577e295de"",
            ""actions"": [
                {
                    ""name"": ""LeftMove"",
                    ""type"": ""Value"",
                    ""id"": ""add89d05-23ef-4ab8-a6dd-3d1dbbdb8b94"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightMove"",
                    ""type"": ""Value"",
                    ""id"": ""47d92d3e-252e-4fc9-9222-1c5fc4dccb33"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""StickDeadzone"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftRotate"",
                    ""type"": ""Value"",
                    ""id"": ""580f3af8-1eab-4c42-a43a-ec966e8b1ceb"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""AxisDeadzone"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightRotate"",
                    ""type"": ""Value"",
                    ""id"": ""4012bc69-d5c7-4419-94b5-ba71a8752d3f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""AxisDeadzone"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftJump"",
                    ""type"": ""Button"",
                    ""id"": ""7e556f27-ba95-4816-af17-31c65b3446c1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightJump"",
                    ""type"": ""Button"",
                    ""id"": ""18d842e5-26f6-4b93-a58d-5ee34055d45a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftCrouch"",
                    ""type"": ""Button"",
                    ""id"": ""48381cef-3c4f-43ad-ba65-316558bd41c3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightCrouch"",
                    ""type"": ""Button"",
                    ""id"": ""bd08ec49-4de0-4997-b128-ef341c761299"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HMDRotation"",
                    ""type"": ""Value"",
                    ""id"": ""fbb3c228-13e5-49c6-9af8-c7986d1748c9"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""4b8fe508-7994-4530-b196-ddc01ab66634"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightHandRotation"",
                    ""type"": ""Value"",
                    ""id"": ""383c75ca-62ac-4d06-83d8-85e795b6fff1"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8851cdca-2635-4bb5-8776-b1ec50fcca5c"",
                    ""path"": ""<OculusTouchController>{LeftHand}/thumbstick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""LeftMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce4354a8-5811-4f9c-a50b-67bd980968f3"",
                    ""path"": ""<OculusTouchController>{RightHand}/thumbstick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""RightRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""033f64da-509c-46f3-aff6-63adc7370d55"",
                    ""path"": ""<OculusTouchController>{RightHand}/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""RightJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd599440-63b5-4940-852d-fcfaca98a151"",
                    ""path"": ""<OculusTouchController>{RightHand}/secondaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""RightCrouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""780ed1ce-7896-47da-ab0c-d1eca73daa24"",
                    ""path"": ""<OculusTouchController>{RightHand}/thumbstick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""RightMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c4eb095-27ee-4d05-b822-bf1c5979ff20"",
                    ""path"": ""<OculusTouchController>{LeftHand}/thumbstick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""LeftRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ccdb6194-854d-4aa5-85f1-da7a9c789f87"",
                    ""path"": ""<OculusTouchController>{LeftHand}/secondaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""LeftCrouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c02eab1-4fe8-41d6-95e6-22697e29be8c"",
                    ""path"": ""<OculusTouchController>{LeftHand}/primaryButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""LeftJump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f538cfc2-dfff-4d24-960a-63cad584913a"",
                    ""path"": ""<OculusHMD>/centerEyeRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""HMDRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ec29ea2-5efe-44b9-b225-1fa487d1316d"",
                    ""path"": ""<OculusTouchController>{LeftHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""LeftHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""119194ba-9200-4670-bb32-94718891a34e"",
                    ""path"": ""<OculusTouchController>{RightHand}/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Oculus"",
                    ""action"": ""RightHandRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Oculus"",
            ""bindingGroup"": ""Oculus"",
            ""devices"": [
                {
                    ""devicePath"": ""<OculusHMD>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<OculusTouchController>{LeftHand}"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<OculusTouchController>{RightHand}"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_LeftMove = m_Gameplay.FindAction("LeftMove", throwIfNotFound: true);
            m_Gameplay_RightMove = m_Gameplay.FindAction("RightMove", throwIfNotFound: true);
            m_Gameplay_LeftRotate = m_Gameplay.FindAction("LeftRotate", throwIfNotFound: true);
            m_Gameplay_RightRotate = m_Gameplay.FindAction("RightRotate", throwIfNotFound: true);
            m_Gameplay_LeftJump = m_Gameplay.FindAction("LeftJump", throwIfNotFound: true);
            m_Gameplay_RightJump = m_Gameplay.FindAction("RightJump", throwIfNotFound: true);
            m_Gameplay_LeftCrouch = m_Gameplay.FindAction("LeftCrouch", throwIfNotFound: true);
            m_Gameplay_RightCrouch = m_Gameplay.FindAction("RightCrouch", throwIfNotFound: true);
            m_Gameplay_HMDRotation = m_Gameplay.FindAction("HMDRotation", throwIfNotFound: true);
            m_Gameplay_LeftHandRotation = m_Gameplay.FindAction("LeftHandRotation", throwIfNotFound: true);
            m_Gameplay_RightHandRotation = m_Gameplay.FindAction("RightHandRotation", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_LeftMove;
        private readonly InputAction m_Gameplay_RightMove;
        private readonly InputAction m_Gameplay_LeftRotate;
        private readonly InputAction m_Gameplay_RightRotate;
        private readonly InputAction m_Gameplay_LeftJump;
        private readonly InputAction m_Gameplay_RightJump;
        private readonly InputAction m_Gameplay_LeftCrouch;
        private readonly InputAction m_Gameplay_RightCrouch;
        private readonly InputAction m_Gameplay_HMDRotation;
        private readonly InputAction m_Gameplay_LeftHandRotation;
        private readonly InputAction m_Gameplay_RightHandRotation;
        public struct GameplayActions
        {
            private @Input m_Wrapper;
            public GameplayActions(@Input wrapper) { m_Wrapper = wrapper; }
            public InputAction @LeftMove => m_Wrapper.m_Gameplay_LeftMove;
            public InputAction @RightMove => m_Wrapper.m_Gameplay_RightMove;
            public InputAction @LeftRotate => m_Wrapper.m_Gameplay_LeftRotate;
            public InputAction @RightRotate => m_Wrapper.m_Gameplay_RightRotate;
            public InputAction @LeftJump => m_Wrapper.m_Gameplay_LeftJump;
            public InputAction @RightJump => m_Wrapper.m_Gameplay_RightJump;
            public InputAction @LeftCrouch => m_Wrapper.m_Gameplay_LeftCrouch;
            public InputAction @RightCrouch => m_Wrapper.m_Gameplay_RightCrouch;
            public InputAction @HMDRotation => m_Wrapper.m_Gameplay_HMDRotation;
            public InputAction @LeftHandRotation => m_Wrapper.m_Gameplay_LeftHandRotation;
            public InputAction @RightHandRotation => m_Wrapper.m_Gameplay_RightHandRotation;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @LeftMove.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftMove;
                    @LeftMove.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftMove;
                    @LeftMove.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftMove;
                    @RightMove.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightMove;
                    @RightMove.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightMove;
                    @RightMove.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightMove;
                    @LeftRotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftRotate;
                    @LeftRotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftRotate;
                    @LeftRotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftRotate;
                    @RightRotate.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightRotate;
                    @RightRotate.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightRotate;
                    @RightRotate.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightRotate;
                    @LeftJump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftJump;
                    @LeftJump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftJump;
                    @LeftJump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftJump;
                    @RightJump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightJump;
                    @RightJump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightJump;
                    @RightJump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightJump;
                    @LeftCrouch.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftCrouch;
                    @LeftCrouch.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftCrouch;
                    @LeftCrouch.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftCrouch;
                    @RightCrouch.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightCrouch;
                    @RightCrouch.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightCrouch;
                    @RightCrouch.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightCrouch;
                    @HMDRotation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHMDRotation;
                    @HMDRotation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHMDRotation;
                    @HMDRotation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHMDRotation;
                    @LeftHandRotation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftHandRotation;
                    @LeftHandRotation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftHandRotation;
                    @LeftHandRotation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLeftHandRotation;
                    @RightHandRotation.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightHandRotation;
                    @RightHandRotation.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightHandRotation;
                    @RightHandRotation.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRightHandRotation;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @LeftMove.started += instance.OnLeftMove;
                    @LeftMove.performed += instance.OnLeftMove;
                    @LeftMove.canceled += instance.OnLeftMove;
                    @RightMove.started += instance.OnRightMove;
                    @RightMove.performed += instance.OnRightMove;
                    @RightMove.canceled += instance.OnRightMove;
                    @LeftRotate.started += instance.OnLeftRotate;
                    @LeftRotate.performed += instance.OnLeftRotate;
                    @LeftRotate.canceled += instance.OnLeftRotate;
                    @RightRotate.started += instance.OnRightRotate;
                    @RightRotate.performed += instance.OnRightRotate;
                    @RightRotate.canceled += instance.OnRightRotate;
                    @LeftJump.started += instance.OnLeftJump;
                    @LeftJump.performed += instance.OnLeftJump;
                    @LeftJump.canceled += instance.OnLeftJump;
                    @RightJump.started += instance.OnRightJump;
                    @RightJump.performed += instance.OnRightJump;
                    @RightJump.canceled += instance.OnRightJump;
                    @LeftCrouch.started += instance.OnLeftCrouch;
                    @LeftCrouch.performed += instance.OnLeftCrouch;
                    @LeftCrouch.canceled += instance.OnLeftCrouch;
                    @RightCrouch.started += instance.OnRightCrouch;
                    @RightCrouch.performed += instance.OnRightCrouch;
                    @RightCrouch.canceled += instance.OnRightCrouch;
                    @HMDRotation.started += instance.OnHMDRotation;
                    @HMDRotation.performed += instance.OnHMDRotation;
                    @HMDRotation.canceled += instance.OnHMDRotation;
                    @LeftHandRotation.started += instance.OnLeftHandRotation;
                    @LeftHandRotation.performed += instance.OnLeftHandRotation;
                    @LeftHandRotation.canceled += instance.OnLeftHandRotation;
                    @RightHandRotation.started += instance.OnRightHandRotation;
                    @RightHandRotation.performed += instance.OnRightHandRotation;
                    @RightHandRotation.canceled += instance.OnRightHandRotation;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);
        private int m_OculusSchemeIndex = -1;
        public InputControlScheme OculusScheme
        {
            get
            {
                if (m_OculusSchemeIndex == -1) m_OculusSchemeIndex = asset.FindControlSchemeIndex("Oculus");
                return asset.controlSchemes[m_OculusSchemeIndex];
            }
        }
        public interface IGameplayActions
        {
            void OnLeftMove(InputAction.CallbackContext context);
            void OnRightMove(InputAction.CallbackContext context);
            void OnLeftRotate(InputAction.CallbackContext context);
            void OnRightRotate(InputAction.CallbackContext context);
            void OnLeftJump(InputAction.CallbackContext context);
            void OnRightJump(InputAction.CallbackContext context);
            void OnLeftCrouch(InputAction.CallbackContext context);
            void OnRightCrouch(InputAction.CallbackContext context);
            void OnHMDRotation(InputAction.CallbackContext context);
            void OnLeftHandRotation(InputAction.CallbackContext context);
            void OnRightHandRotation(InputAction.CallbackContext context);
        }
    }
}
