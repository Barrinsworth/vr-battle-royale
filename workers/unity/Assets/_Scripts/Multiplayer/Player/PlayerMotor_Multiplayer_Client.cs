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
        [SerializeField] private Mover mover;
        [SerializeField] private Rigidbody moverRigidbody;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private float movementUpdateFrequency = 0.03333f;

        private PlayerMotorStateEnum currentMotorState = PlayerMotorStateEnum.Falling;
        private Vector3 momentum = Vector3.zero;
        private Vector3 savedVelocity = Vector3.zero;
        private Vector3 savedMovementVelocity = Vector3.zero;
        private Vector3 savedPlayerLocalPosition = Vector3.zero;
        private Vector3 smoothVelocity = Vector3.zero;
        private Vector3 origin = Vector3.zero;
        private float jumpStartTime = 0f;
        private float snapRotationTime = 0f;
        private float lastGroundedTime = 0f;
        private float timeSinceLastMovementUpdate = 0f;
        private bool crouching = false;
        private bool jumpPressed = false;

        private Vector3 clientMovementUpdateMovementInput = Vector3.zero;
        private Vector3 clientMovementUpdatePlayerDelta = Vector3.zero;
        private bool clientMovementUpdateJump = false;
        private bool clientMovementUpdateCrouch = false;

        public bool IsGrounded { get { return (currentMotorState == PlayerMotorStateEnum.Grounded || currentMotorState == PlayerMotorStateEnum.Sliding); } }
        private float PlayerHeight { get { return Camera.main.transform.localPosition.y - (crouching == true ? motorVariables.CrouchDistance : 0f); } }
        private Vector3 DesiredHeadPosition
        {
            get
            {
                var cameraForwardXZ = Camera.main.transform.forward;
                cameraForwardXZ.y = 0f;

                return mover.transform.position + (Vector3.up * (mover.colliderHeight - headCollider.radius)) + (cameraForwardXZ.normalized * motorVariables.CameraForwardOffset);
            }
        }

        public delegate void PlayerMotorVector3Event(Vector3 v);
        public event PlayerMotorVector3Event OnJump;
        public event PlayerMotorVector3Event OnLand;

        #region Unity Life Cycle
        private void OnEnable()
        {
            origin = GetComponent<LinkedEntityComponent>().Worker.Origin;
        }

        private void Start()
        {
            savedPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void Update()
        {
            TeleportPlayerHead(Vector3.SmoothDamp(Camera.main.transform.position, DesiredHeadPosition, ref smoothVelocity, motorVariables.CameraSmoothTime));

            GetInput();

            HandleRotation();

            timeSinceLastMovementUpdate += Time.deltaTime;

            if (timeSinceLastMovementUpdate >= movementUpdateFrequency)
            {
                SendClientMovementUpdate();

                timeSinceLastMovementUpdate = 0f;
            }
        }

        private void FixedUpdate()
        {
            var deltaPlayerLocalPosition = Camera.main.transform.localPosition - savedPlayerLocalPosition;
            deltaPlayerLocalPosition.y = 0f;

            var deltaWorldCoords = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.TransformDirection(deltaPlayerLocalPosition);

            clientMovementUpdatePlayerDelta += deltaWorldCoords;

            moverRigidbody.MovePosition(moverRigidbody.transform.position + deltaWorldCoords);

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

            if (velocity.x != 0f || velocity.y != 0f || velocity.z != 0)
            {
                PlayerController_Multiplayer_Client.Instance.CurrentVRRig.FOVBlinders.FadeBlindersIn();
            }
            else
            {
                PlayerController_Multiplayer_Client.Instance.CurrentVRRig.FOVBlinders.FadeBlindersOut();
            }

            savedVelocity = velocity;
            savedMovementVelocity = velocity - momentum;

            ResetInput();

            savedPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void OnDisable()
        {
            
        }

        private void OnDestroy()
        {
            OnJump = null;
            OnLand = null;
        }
        #endregion

        private void GetInput()
        {
            var jumpInput = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.JumpButtonPressed;

            if(!jumpPressed)
            {
                jumpPressed = jumpInput;
            }

            if (!clientMovementUpdateJump)
            {
                clientMovementUpdateJump = jumpInput;
            }

            var crouchInput = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.CrouchButtonPressed;

            if(crouchInput)
            {
                if (crouching)
                {
                    UnCrouch();
                }
                else
                {
                    Crouch();
                }

                clientMovementUpdateCrouch = !clientMovementUpdateCrouch;
            }
        }

        private void SendClientMovementUpdate()
        {
            var update = new ClientPlayerMovementUpdate.Update();
            update.MovementInput = Vector2Util.ConvertToSpatialOSVector2(new Vector2(clientMovementUpdateMovementInput.x, clientMovementUpdateMovementInput.z));
            update.PlayerDelta = Vector2Util.ConvertToSpatialOSVector2(new Vector2(clientMovementUpdatePlayerDelta.x, clientMovementUpdatePlayerDelta.z));
            update.RotationInput = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.eulerAngles.y;
            update.Crouch = clientMovementUpdateCrouch;
            update.Jump = clientMovementUpdateJump;

            clientMovementWriter.SendUpdate(update);

            clientMovementUpdateMovementInput = Vector3.zero;
            clientMovementUpdatePlayerDelta = Vector3.zero;
            clientMovementUpdateJump = false;
            clientMovementUpdateCrouch = false;
            
        }

        private void ResetInput()
        {
            jumpPressed = false;
        }

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

        private void HandleRotation()
        {
            var rotationInput = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.RotationInput;

            if (rotationInput == 0f)
            {
                return;
            }

            var deltaRotation = 0f;

            if (PlayerSettingsController.Instance.RotationMode == RotationModeEnum.Smooth)
            {
                deltaRotation = (rotationInput * (float)PlayerSettingsController.Instance.SmoothRotationSpeed * motorVariables.SmoothRotationMultiplier * Time.fixedDeltaTime);
            }
            else
            {
                if (Time.time - snapRotationTime >= motorVariables.SnapRotationCooldown)
                {
                    snapRotationTime = Time.time;

                    deltaRotation = rotationInput > 0 ? PlayerSettingsController.Instance.SnapRotationDegrees : -PlayerSettingsController.Instance.SnapRotationDegrees;
                }
            }

            if (deltaRotation != 0f)
            {
                var yEulerAngle = 0f;

                if (PlayerSettingsController.Instance.RoomSetup == RoomSetupEnum.Roomscale)
                {
                    yEulerAngle = Camera.main.transform.eulerAngles.y + deltaRotation;
                }
                else
                {
                    yEulerAngle = PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.eulerAngles.y + deltaRotation;
                }

                TeleportPlayerHead(Camera.main.transform.position, yEulerAngle);
            }
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
                        GroundContactRegained(momentum);
                        break;
                    }

                    if (isSliding)
                    {
                        currentMotorState = PlayerMotorStateEnum.Sliding;
                        GroundContactRegained(momentum);
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
                        GroundContactRegained(momentum);
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
                        GroundContactRegained(momentum);
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
                if (jumpPressed)
                {
                    GroundContactLost();

                    JumpStart();

                    currentMotorState = PlayerMotorStateEnum.Jumping;
                }
            }
        }

        protected Vector3 CalculateMovementVelocity()
        {
            var velocity = CalculateMovementDirection() * Time.fixedDeltaTime;

            clientMovementUpdateMovementInput += velocity;

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

        private Vector3 CalculateMovementDirection()
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

            return rotatedMovementInput;
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

        private void GroundContactRegained(Vector3 collisionVelocity)
        {
            if (OnLand != null)
                OnLand(collisionVelocity);
        }

        private void JumpStart()
        {
            momentum += mover.transform.up * motorVariables.JumpSpeed;

            jumpStartTime = Time.time;

            if (OnJump != null)
            {
                OnJump(momentum);
            }
        }

        private void Crouch()
        {
            crouching = true;
        }

        private void UnCrouch()
        {
            crouching = false;
        }

        #region Teleports
        private void TeleportPlayerRoom(Vector3 desiredWorldPositionOfRoom, Quaternion desiredWordRotationOfRoom)
        {
            PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.rotation = desiredWordRotationOfRoom;
            PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.position = desiredWorldPositionOfRoom;
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera)
        {
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.position - Camera.main.transform.position),
                PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.rotation);
        }

        private void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera, float lookAtYEulerAngle)
        {
            if (PlayerSettingsController.Instance.RoomSetup == RoomSetupEnum.Roomscale)
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.eulerAngles.y, 0f) *
                    (PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.position - Camera.main.transform.position)),
                    Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.localEulerAngles.y, 0f));
            }
            else
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.eulerAngles.y, 0f) *
                    (PlayerController_Multiplayer_Client.Instance.CurrentVRRig.transform.position - Camera.main.transform.position)),
                    Quaternion.Euler(0f, lookAtYEulerAngle, 0f));
            }
        }
        #endregion
    }
}
