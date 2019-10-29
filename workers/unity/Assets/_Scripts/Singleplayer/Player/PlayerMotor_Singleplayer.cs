using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;

namespace VRBattleRoyale.Singleplayer
{
    public class PlayerMotor_Singleplayer : MonoBehaviour
    {
        [Header("--Player Motor Single Player--")]
        [SerializeField] private PlayerMotorVariables motorVariables;
        [SerializeField] private Mover mover;
        [SerializeField] private Rigidbody moverRigidbody;
        [SerializeField] private SphereCollider headCollider;

        private PlayerMotorStateEnum currentMotorState = PlayerMotorStateEnum.Falling;
        private Vector3 momentum = Vector3.zero;
        private Vector3 savedVelocity = Vector3.zero;
        private Vector3 savedMovementVelocity = Vector3.zero;
        private Vector3 savedPlayerLocalPosition = Vector3.zero;
        private Vector3 smoothVelocity = Vector3.zero;
        private float jumpStartTime = 0f;
        private float snapRotationTime = 0f;
        private float lastGroundedTime = 0f;
        private bool crouching = false;

        private bool jumpPressed = false;
        private bool crouchPressed = false;

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
        private void Update()
        {
            TeleportPlayerHead(Vector3.SmoothDamp(Camera.main.transform.position, DesiredHeadPosition, ref smoothVelocity, motorVariables.CameraSmoothTime));

            GetInput();

            HandleRotation();
        }

        private void FixedUpdate()
        {
            var deltaPlayerLocalPosition = Camera.main.transform.localPosition - savedPlayerLocalPosition;
            deltaPlayerLocalPosition.y = 0f;

            moverRigidbody.MovePosition(moverRigidbody.transform.position + (Quaternion.Euler(0f, PlayerController_Singleplayer.Instance.YEulerAngle, 0f) *
                new Vector3(deltaPlayerLocalPosition.x, 0f, deltaPlayerLocalPosition.z)));

            moverRigidbody.MoveRotation(PlayerController_Singleplayer.Instance.Rotation);

            headCollider.transform.position = DesiredHeadPosition;

            ResizeMover();

            mover.CheckForGround();

            HandleState();

            HandleMomentum();

            HandleJumping();

            HandleCrouching();

            var velocity = CalculateMovementVelocity();

            velocity += momentum;

            mover.SetExtendSensorRange(IsGrounded);

            mover.SetVelocity(velocity);

            if (velocity.x != 0f || velocity.y != 0f || velocity.z != 0)
            {
                PlayerController_Singleplayer.Instance.CurrentVRRig.FOVBlinders.FadeBlindersIn();
            }
            else
            {
                PlayerController_Singleplayer.Instance.CurrentVRRig.FOVBlinders.FadeBlindersOut();
            }

            savedVelocity = velocity;
            savedMovementVelocity = velocity - momentum;

            ResetInput();

            savedPlayerLocalPosition = Camera.main.transform.localPosition;
        }

        private void OnDestroy()
        {
            OnJump = null;
            OnLand = null;
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

            mover.colliderHeight = Mathf.Max(newMoverColliderHeight, mover.colliderThickness);

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

        private void GetInput()
        {
            if (!jumpPressed)
            {
                jumpPressed = PlayerController_Singleplayer.Instance.CurrentVRRig.JumpButtonPressed;
            }

            if (!crouchPressed)
            {
                crouchPressed = PlayerController_Singleplayer.Instance.CurrentVRRig.CrouchButtonPressed;
            }
        }

        private void ResetInput()
        {
            jumpPressed = false;
            crouchPressed = false;
        }

        private void HandleRotation()
        {
            var rotationInput = PlayerController_Singleplayer.Instance.CurrentVRRig.RotationInput;

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
                if (PlayerSettingsController.Instance.RoomSetup == RoomSetupEnum.Roomscale)
                {
                    TeleportPlayerHead(Camera.main.transform.position, Camera.main.transform.eulerAngles.y + deltaRotation);
                }
                else
                {
                    TeleportPlayerHead(Camera.main.transform.position, PlayerController_Singleplayer.Instance.YEulerAngle + deltaRotation);
                }
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

        private void HandleCrouching()
        {
            if (crouching)
            {
                if ((currentMotorState != PlayerMotorStateEnum.Grounded && currentMotorState != PlayerMotorStateEnum.Sliding) ||
                    crouchPressed)
                {
                    UnCrouch();
                }
            }
            else
            {
                if (crouchPressed && (currentMotorState == PlayerMotorStateEnum.Grounded ||
                    currentMotorState == PlayerMotorStateEnum.Sliding))
                {
                    Crouch();
                }
            }
        }

        protected Vector3 CalculateMovementVelocity()
        {
            var velocityDirection = CalculateMovementDirection();

            var velocity = velocityDirection;

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
                yRotation = PlayerController_Singleplayer.Instance.CurrentVRRig.MoveHand.eulerAngles.y;
            }
            else
            {
                yRotation = Camera.main.transform.eulerAngles.y;
            }

            var movementInput = PlayerController_Singleplayer.Instance.CurrentVRRig.MovementInput;
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
        public void TeleportPlayerRoom(Vector3 desiredWorldPositionOfRoom, Quaternion desiredWordRotationOfRoom)
        {
            PlayerController_Singleplayer.Instance.Rotation = desiredWordRotationOfRoom;
            PlayerController_Singleplayer.Instance.Position = desiredWorldPositionOfRoom;
        }

        public void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera)
        {
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (PlayerController_Singleplayer.Instance.Position - Camera.main.transform.position), PlayerController_Singleplayer.Instance.Rotation);
        }

        public void TeleportPlayerFeet(Vector3 desiredWorldPositionOfPlayerFeet)
        {
            TeleportPlayerHead(desiredWorldPositionOfPlayerFeet + (Vector3.up * PlayerHeight));
        }

        public void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera, float lookAtYEulerAngle)
        {
            if (PlayerSettingsController.Instance.RoomSetup == RoomSetupEnum.Roomscale)
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.eulerAngles.y, 0f) *
                    (PlayerController_Singleplayer.Instance.Position - Camera.main.transform.position)),
                    Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.localEulerAngles.y, 0f));
            }
            else
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - PlayerController_Singleplayer.Instance.YEulerAngle, 0f) *
                    (PlayerController_Singleplayer.Instance.Position - Camera.main.transform.position)),
                    Quaternion.Euler(0f, lookAtYEulerAngle, 0f));
            }
        }

        public void TeleportPlayerFeet(Vector3 desiredWorldPositionOfPlayerFeet, float lookAtYEulerAngle)
        {
            TeleportPlayerHead(desiredWorldPositionOfPlayerFeet + (Vector3.up * PlayerHeight), lookAtYEulerAngle);
        }
        #endregion
    }
}
