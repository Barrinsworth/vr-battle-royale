using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;
using Vitruvius.Generated.Player;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityClient, WorkerUtils.MobileClient)]
    public class PlayerMotor_Multiplayer_Client : PlayerMotor_Multiplayer
    {
        [Require] private ClientPlayerMovementUpdateWriter clientMovementWriter;

        [Header("--Player Motor Multiplayer Client--")]
        [SerializeField] private PlayerMotorVariables motorVariables;
        [SerializeField] private float movementUpdateFrequency = 0.03333f;

        private Vector3 origin = Vector3.zero;
        private float timeSinceLastMovementUpdate = 0f;

        private Vector3 clientMovementUpdateMovementInput = Vector3.zero;
        private bool clientMovementUpdateJump = false;
        private bool clientMovementUpdateCrouch = false;

        #region Unity Life Cycle
        private void OnEnable()
        {
            origin = GetComponent<LinkedEntityComponent>().Worker.Origin;
        }

        private void Update()
        {
            timeSinceLastMovementUpdate += Time.deltaTime;

            GetInput();

            if (timeSinceLastMovementUpdate >= movementUpdateFrequency)
            {
                SendClientMovementUpdate();

                ResetInput();

                timeSinceLastMovementUpdate = 0f;
            }
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
        }

        private void ResetInput()
        {
            clientMovementUpdateJump = false;
            clientMovementUpdateCrouch = false;
            clientMovementUpdateMovementInput = Vector3.zero;
        }
    }
}
