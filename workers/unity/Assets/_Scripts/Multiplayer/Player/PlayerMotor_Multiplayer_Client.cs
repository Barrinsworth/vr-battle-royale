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
        private bool crouching = false;

        private Vector3 clientMovementUpdatePlayerPhysicalDelta = Vector3.zero;
        private Vector3 clientMovementUpdateMovementInput = Vector3.zero;
        private float clientMovementUpdateTimeDelta = 0f;
        private int clientMovementUpdateMessageStamp = 1;
        private bool clientMovementUpdateJump = false;

        private float PlayerHeight { get { return Camera.main.transform.localPosition.y - (crouching == true ? motorVariables.CrouchDistance : 0f); } }

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
            clientMovementUpdateTimeDelta += Time.deltaTime;

            GetInput();

            if (clientMovementUpdateTimeDelta >= updateFrequency)
            {
                SendClientMovementUpdate();
            }

            previousPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void OnDisable()
        {
            
        }
        #endregion

        private void GetInput()
        {
            clientMovementUpdatePlayerPhysicalDelta += Camera.main.transform.localPosition - previousPlayerLocalPosition;

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

            clientMovementUpdateMovementInput += rotatedMovementInput;

            if (!clientMovementUpdateJump)
            {
                clientMovementUpdateJump = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.JumpButtonPressed;
            }

            if(PlayerController_Multiplayer_Client.Instance.CurrentVRRig.CrouchButtonPressed)
            {
                crouching = !crouching;
            }
        }

        private void SendClientMovementUpdate()
        {
            var update = new ClientPlayerMovementUpdate.Update();
            update.TimeDelta = clientMovementUpdateTimeDelta;
            update.Height = PlayerHeight;
            update.Jump = clientMovementUpdateJump;
            update.MessageStamp = (uint)clientMovementUpdateMessageStamp;
            update.MovementInput = Vector2Util.ConvertToSpatialOSVector2(new Vector2(clientMovementUpdateMovementInput.x, clientMovementUpdateMovementInput.z));
            update.PlayerPhysicalDelta = Vector2Util.ConvertToSpatialOSVector2(new Vector2(clientMovementUpdatePlayerPhysicalDelta.x, clientMovementUpdatePlayerPhysicalDelta.z));

            clientMovementWriter.SendUpdate(update);
            clientMovementUpdateMessageStamp++;

            ResetClientMovementUpdate();
        }

        private void ResetClientMovementUpdate()
        {
            clientMovementUpdateTimeDelta = 0f;
            clientMovementUpdateJump = false;
            clientMovementUpdateMovementInput = Vector3.zero;
            clientMovementUpdatePlayerPhysicalDelta = Vector3.zero;
        }
    }
}
