using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;
using Vitruvius.Generated.Player;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient)]
    public class PlayerMotor_Multiplayer_Client : PlayerMotor_Multiplayer
    {
        [Require] private ClientPlayerMovementUpdateWriter clientMovementWriter;

        [Header("--Player Motor Multiplayer Client--")]
        [SerializeField] private PlayerMotorVariables motorVariables;
        [SerializeField] private float updateFrequency = 0.03333f;

        private Vector3 previousPlayerLocalPosition = Vector3.zero;
        private float timeSinceLastUpdate = 0f;

        private Vector3 clientMovementUpdateMovementInput = Vector3.zero;
        private bool clientMovementUpdateJump = false;
        private bool clientMovementUpdateCrouch = false;

        #region Unity Life Cycle
        private void Awake()
        {
            previousPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void OnEnable()
        {
            
        }

        private void Update()
        {
            timeSinceLastUpdate += Time.deltaTime;

            GetInput();

            if (timeSinceLastUpdate >= updateFrequency)
            {
                SendClientMovementUpdate();

                timeSinceLastUpdate = 0f;
            }

            previousPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void OnDisable()
        {
            
        }
        #endregion

        private void GetInput()
        {
            var direction = Vector3.zero;
            var yRotation = 0f;

            if (PlayerSettingsController.Instance.MovementOrientationMode == MovementOrientationModeEnum.Hand)
            {
                yRotation = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.MoveHand.eulerAngles.y;
            }
            else
            {
                yRotation = Camera.main.transform.eulerAngles.y;
            }

            var movementInput = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.MovementInput;
            var rotatedMovementInput = Quaternion.Euler(0f, yRotation, 0f) * new Vector3(movementInput.x, 0f, movementInput.y);

            if (rotatedMovementInput.magnitude > 1f)
                rotatedMovementInput.Normalize();

            clientMovementUpdateMovementInput += rotatedMovementInput * motorVariables.MovementSpeed * Time.fixedDeltaTime;

            if (!clientMovementUpdateJump)
            {
                clientMovementUpdateJump = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.JumpButtonPressed;
            }

            if (!clientMovementUpdateCrouch)
            {
                clientMovementUpdateCrouch = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.CrouchButtonPressed;
            }
        }

        private void SendClientMovementUpdate()
        {
            var update = new ClientPlayerMovementUpdate.Update();
            update.MovementInput = Vector2Util.ConvertToSpatialOSVector2(new Vector2(clientMovementUpdateMovementInput.x, clientMovementUpdateMovementInput.z));
            update.Crouch = clientMovementUpdateCrouch;
            update.Jump = clientMovementUpdateJump;

            clientMovementWriter.SendUpdate(update);

            ResetClientMovementUpdate();
        }

        private void ResetClientMovementUpdate()
        {
            clientMovementUpdateJump = false;
            clientMovementUpdateMovementInput = Vector3.zero;
        }
    }
}
