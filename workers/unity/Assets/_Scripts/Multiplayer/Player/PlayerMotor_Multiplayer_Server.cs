using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable;
using Improbable.Gdk.Subscriptions;
using VRBattleRoyale.Common.Player;
using Vitruvius.Generated.Player;

namespace VRBattleRoyale.Multiplayer
{
    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class PlayerMotor_Multiplayer_Server : PlayerMotor_Multiplayer
    {
        // 4 frames at 60FPS
        private static float MAX_CLIENT_MOVEMENT_INPUT = 0.06666f;
        private static float MAX_CLIENT_PLAYER_DELTA = 0.1f;

        [Require] private ClientPlayerMovementUpdateReader clientMovementReader;
        [Require] private PositionWriter spatialPosition;

        [Header("--Player Motor Multiplayer Server--")]
        [SerializeField] private PlayerController_Multiplayer_Proxy playerControllerProxy;
        [SerializeField] private PlayerMotorVariables motorVariables;
        [SerializeField] private Mover mover;
        [SerializeField] private Rigidbody moverRigidbody;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private float spatialOSUpdateFrequency = 1f;

        private PlayerMotorStateEnum currentMotorState = PlayerMotorStateEnum.Falling;
        private Vector3 momentum = Vector3.zero;
        private Vector3 savedVelocity = Vector3.zero;
        private Vector3 savedMovementVelocity = Vector3.zero;
        private Vector3 origin = Vector3.zero;
        private float jumpStartTime = 0f;
        private float lastGroundedTime = 0f;
        private float timeSinceLastSpatialOSUpdate = 0f;
        private bool crouching = false;

        private Vector2 movementInput = Vector2.zero;
        private Vector2 playerDelta = Vector2.zero;
        private float rigForward = 0f;
        private bool jump = false;

        public bool IsGrounded { get { return (currentMotorState == PlayerMotorStateEnum.Grounded || currentMotorState == PlayerMotorStateEnum.Sliding); } }
        private float PlayerHeight { get { return playerControllerProxy.HMD.localPosition.y - (crouching == true ? motorVariables.CrouchDistance : 0f); } }
        private Vector3 DesiredHeadPosition
        {
            get
            {
                var cameraForwardXZ = playerControllerProxy.HMD.forward;
                cameraForwardXZ.y = 0f;

                return mover.transform.position + (Vector3.up * (mover.colliderHeight - headCollider.radius)) + (cameraForwardXZ.normalized * motorVariables.CameraForwardOffset);
            }
        }

        #region Unity Life Cycle
        private void OnEnable()
        {
            origin = GetComponent<LinkedEntityComponent>().Worker.Origin;

            clientMovementReader.OnUpdate += PlayerMovementUpdate_Client;
        }

        private void Update()
        {
            TeleportPlayerHead(DesiredHeadPosition, rigForward);

            timeSinceLastSpatialOSUpdate += Time.deltaTime;

            if(timeSinceLastSpatialOSUpdate >= spatialOSUpdateFrequency)
            {
                var positionUpdate = new Position.Update { Coords = Coordinates.FromUnityVector(mover.transform.position - origin) };
                spatialPosition.SendUpdate(positionUpdate);

                timeSinceLastSpatialOSUpdate = 0f;
            }
        }

        private void FixedUpdate()
        {
            moverRigidbody.MovePosition(moverRigidbody.transform.position + new Vector3(playerDelta.x, 0f, playerDelta.y));

            headCollider.transform.position = DesiredHeadPosition;

            ResizeMover();

            mover.CheckForGround();

            HandleState();

            HandleMomentum();

            HandleJumping();

            var velocity = CalculateMovementVelocity();

            velocity += momentum;

            mover.SetExtendSensorRange(IsGrounded);

            mover.SetVelocity(velocity);

            savedVelocity = velocity;
            savedMovementVelocity = velocity - momentum;

            movementInput = Vector2.zero;
            playerDelta = Vector2.zero;
            jump = false;
        }

        private void OnDisable()
        {
            clientMovementReader.OnUpdate -= PlayerMovementUpdate_Client;
        }
        #endregion

        #region SpatialOS Event Listeners
        private void PlayerMovementUpdate_Client(ClientPlayerMovementUpdate.Update update)
        {
            var clientMovementInput = Vector2Util.ConvertToUnityVector2(update.MovementInput);

            if(clientMovementInput.magnitude > MAX_CLIENT_MOVEMENT_INPUT)
            {
                clientMovementInput = clientMovementInput.normalized * MAX_CLIENT_MOVEMENT_INPUT;
            }
 
            movementInput = clientMovementInput;

            var clientPlayerDelta = Vector2Util.ConvertToUnityVector2(update.PlayerDelta);

            if (clientPlayerDelta.magnitude > MAX_CLIENT_PLAYER_DELTA)
            {
                clientPlayerDelta = clientPlayerDelta.normalized * MAX_CLIENT_PLAYER_DELTA;
            }

            playerDelta = clientPlayerDelta;

            rigForward = update.RotationInput;
            jump = update.Jump;

            if(update.Crouch)
            {
                crouching = !crouching;
            }
        }
        #endregion

        private void ResizeMover()
        {
            var newMoverColliderHeight = PlayerHeight + headCollider.radius;

            var raycastHit = new RaycastHit();
            if (Physics.Linecast(mover.transform.position, mover.transform.position + (Vector3.up * newMoverColliderHeight), out raycastHit, mover.sensorLayermask))
            {
                newMoverColliderHeight = raycastHit.distance - 0.01f;
            }

            mover.colliderHeight = Mathf.Max(newMoverColliderHeight, headCollider.radius * 2f);

            if (mover.colliderHeight >= motorVariables.StepHeightWorldUnits + mover.colliderThickness)
            {
                mover.stepHeight = motorVariables.StepHeightWorldUnits / mover.colliderHeight;
            }
            else
            {
                mover.stepHeight = Mathf.Max((mover.colliderHeight - motorVariables.StepHeightWorldUnits) /
                    mover.colliderHeight, 0f);
            }

            mover.RecalculateColliderDimensions();
        }

        private void HandleState()
        {
            var isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(momentum, mover.transform.up) > 0f);
            var isSliding = mover.IsGrounded() &&
                (Vector3.Angle(mover.GetGroundNormal(), mover.transform.up) > motorVariables.SlopeLimit);

            switch (currentMotorState)
            {
                case PlayerMotorStateEnum.Grounded:

                    if (isRising)
                    {
                        currentMotorState = PlayerMotorStateEnum.Rising;
                        GroundContactLost();
                        break;
                    }

                    if (!mover.IsGrounded())
                    {
                        currentMotorState = PlayerMotorStateEnum.Falling;
                        GroundContactLost();
                        break;
                    }

                    if (isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Sliding;
                        break;
                    }

                    lastGroundedTime = Time.time;

                    break;

                case PlayerMotorStateEnum.Falling:

                    if (isRising)
                    {
                        currentMotorState = PlayerMotorStateEnum.Rising;
                        break;
                    }

                    if (mover.IsGrounded() && !isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Grounded;
                        break;
                    }

                    if (isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Sliding;
                        break;
                    }

                    break;

                case PlayerMotorStateEnum.Sliding:

                    if (isRising)
                    {
                        currentMotorState = PlayerMotorStateEnum.Rising;
                        GroundContactLost();
                        break;
                    }

                    if (!mover.IsGrounded())
                    {
                        currentMotorState = PlayerMotorStateEnum.Falling;
                        break;
                    }
                    if (mover.IsGrounded() && !isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Grounded;
                        break;
                    }
                    break;

                case PlayerMotorStateEnum.Rising:

                    if (isRising)
                        break;

                    if (mover.IsGrounded() && !isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Grounded;
                        break;
                    }

                    if (isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Sliding;
                        break;
                    }

                    if (!mover.IsGrounded())
                    {
                        currentMotorState = PlayerMotorStateEnum.Falling;
                        break;
                    }

                    break;

                case PlayerMotorStateEnum.Jumping:

                    if ((Time.time - jumpStartTime) > motorVariables.JumpDuration)
                    {
                        currentMotorState = PlayerMotorStateEnum.Rising;
                        break;
                    }

                    break;
            }
        }

        private void HandleMomentum()
        {
            var verticalMomentum = Vector3.zero;
            var horizontalMomentum = Vector3.zero;

            if (momentum != Vector3.zero)
            {
                verticalMomentum = VectorMath.ExtractDotVector(momentum, mover.transform.up);
                horizontalMomentum = momentum - verticalMomentum;
            }

            if (currentMotorState == PlayerMotorStateEnum.Sliding)
                verticalMomentum -= mover.transform.up * motorVariables.SlideGravity * Time.fixedDeltaTime;
            else
                verticalMomentum -= mover.transform.up * motorVariables.Gravity * Time.fixedDeltaTime;

            if (currentMotorState == PlayerMotorStateEnum.Grounded)
                verticalMomentum = Vector3.zero;

            if (IsGrounded)
                horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(horizontalMomentum, motorVariables.GroundFriction, Time.fixedDeltaTime, 0f);
            else
                horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(horizontalMomentum, motorVariables.AirFriction, Time.fixedDeltaTime, 0f);

            momentum = horizontalMomentum + verticalMomentum;

            if (currentMotorState == PlayerMotorStateEnum.Sliding)
            {
                momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
            }

            if (currentMotorState == PlayerMotorStateEnum.Jumping)
            {
                momentum = VectorMath.RemoveDotVector(momentum, mover.transform.up);
                momentum += mover.transform.up * motorVariables.JumpSpeed;
            }
        }

        private void HandleJumping()
        {
            if (currentMotorState == PlayerMotorStateEnum.Grounded || ((currentMotorState == PlayerMotorStateEnum.Falling || currentMotorState == PlayerMotorStateEnum.Sliding) &&
                Time.time - lastGroundedTime < motorVariables.ExtraJumpTimeAfterLeavingGround))
            {
                if (jump)
                {
                    GroundContactLost();

                    JumpStart();

                    currentMotorState = PlayerMotorStateEnum.Jumping;
                }
            }
        }

        protected Vector3 CalculateMovementVelocity()
        {
            var velocity = new Vector3(movementInput.x, 0f, movementInput.y);

            velocity *= motorVariables.MovementSpeed;

            if (!IsGrounded)
                velocity *= motorVariables.AirControl;

            if (currentMotorState == PlayerMotorStateEnum.Sliding)
            {
                var _factor = Mathf.InverseLerp(90f, 0f, Vector3.Angle(mover.transform.up, mover.GetGroundNormal()));
                velocity *= _factor;
            }

            return velocity;
        }

        private bool IsRisingOrFalling()
        {
            var verticalMomentum = VectorMath.ExtractDotVector(momentum, mover.transform.up);

            return (verticalMomentum.magnitude > 0.001f);
        }

        private void GroundContactLost()
        {
            var horizontalMomentumSpeed = VectorMath.RemoveDotVector(momentum, mover.transform.up).magnitude;
            var currentVelocity = momentum + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(motorVariables.MovementSpeed - horizontalMomentumSpeed, 0f, motorVariables.MovementSpeed));

            var length = currentVelocity.magnitude;

            var velocityDirection = Vector3.zero;
            if (length != 0f)
            {
                velocityDirection = currentVelocity / length;
            }

            if (length >= motorVariables.MovementSpeed * motorVariables.AirControl)
            {
                length -= motorVariables.MovementSpeed * motorVariables.AirControl;
            }
            else
            {
                length = 0f;
            }

            momentum = velocityDirection * length;
        }

        private void JumpStart()
        {
            momentum += mover.transform.up * motorVariables.JumpSpeed;

            jumpStartTime = Time.time;
        }

        #region Teleports
        private void TeleportPlayerRoom(Vector3 desiredWorldPositionOfRoom, Quaternion desiredWordRotationOfRoom)
        {
            playerControllerProxy.Rig.rotation = desiredWordRotationOfRoom;
            playerControllerProxy.Rig.position = desiredWorldPositionOfRoom;
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera)
        {
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (playerControllerProxy.Rig.position - playerControllerProxy.HMD.position), playerControllerProxy.Rig.rotation);
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera, float lookAtYEulerAngle)
        {
            Debug.Log(lookAtYEulerAngle);
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (playerControllerProxy.Rig.position - playerControllerProxy.HMD.position), Quaternion.Euler(0f, lookAtYEulerAngle, 0f));
        }
        #endregion
    }
}
