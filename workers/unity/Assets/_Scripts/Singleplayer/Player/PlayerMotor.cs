using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;
using VRBattleRoyale.Common.Player;

namespace VRBattleRoyale.SinglePlayer
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("--Player Motor--")]
        [SerializeField] private Mover mover;
        [SerializeField] private Rigidbody moverRigidbody;
        [SerializeField] private SphereCollider headCollider;
        [SerializeField] private float movementSpeed = 7f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float jumpDuration = 0.2f;
        [SerializeField] private float stepHeightWorldUnits = 0.4f;
        [SerializeField] private int slopeLimit = 70;
        [SerializeField] [Range(0f, 1f)] private float airControl = 0.4f;
        [SerializeField] private float gravity = 30f;
        [SerializeField] private float slideGravity = 30f;
        [SerializeField] private float airFriction = 0.5f;
        [SerializeField] private float groundFriction = 100f;
        [SerializeField] private float smoothRotationMultiplier = 30;
        [SerializeField] private float snapRotationCooldown = 0.3f;
        [SerializeField] private float crouchDistance = 0.5f;
        [SerializeField] private float cameraForwardOffset = -0.05f;

        private PlayerMotorStateEnum currentMotorState = PlayerMotorStateEnum.Falling;
        private Vector3 momentum = Vector3.zero;
        private Vector3 savedVelocity = Vector3.zero;
        private Vector3 savedMovementVelocity = Vector3.zero;
        private Vector3 savedMoverPosition = Vector3.zero;
        private Vector3 savedPlayerLocalPosition = Vector3.zero;
        private float jumpStartTime = 0f;
        private float snapRotationTime = 0f;
        private float crouchTime = 0f;
        private bool crouching = false;

        private bool jumpPressed = false;
        private bool crouchPressed = false;

        public bool IsGrounded { get { return (currentMotorState == PlayerMotorStateEnum.Grounded || currentMotorState == PlayerMotorStateEnum.Sliding); } }
        public float PlayerHeight { get { return Camera.main.transform.localPosition.y - (crouching == true ? crouchDistance : 0f); } }

        //Events;
        public delegate void PlayerMotorVector3Event(Vector3 v);
        public event PlayerMotorVector3Event OnJump;
        public event PlayerMotorVector3Event OnLand;

        #region Unity Life Cycle
        private void Awake()
        {
            mover.transform.parent = null;
        }

        private void Update()
        {
            var cameraForwardXZ = Camera.main.transform.forward;
            cameraForwardXZ.y = 0f;

            TeleportPlayerHead(mover.transform.position + (Vector3.up * (mover.colliderHeight - headCollider.radius)) + (cameraForwardXZ.normalized * cameraForwardOffset));

            GetInput();

            HandleRotation();    
        }

        private void FixedUpdate()
        {
            var deltaPlayerLocalPosition = Camera.main.transform.localPosition - savedPlayerLocalPosition;
            deltaPlayerLocalPosition.y = 0f;

            moverRigidbody.MovePosition(moverRigidbody.transform.position + (Quaternion.Euler(0f, PlayerController.Instance.transform.eulerAngles.y, 0f) *
                new Vector3(deltaPlayerLocalPosition.x, 0f, deltaPlayerLocalPosition.z)));

            headCollider.transform.position = Camera.main.transform.position;

            //PlayerController.Instance.transform.position += mover.transform.position - savedMoverPosition;
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

            savedVelocity = velocity;
            savedMovementVelocity = velocity - momentum;

            //savedMoverPosition = mover.transform.position;

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
            if(Physics.Linecast(mover.transform.position, mover.transform.position + (Vector3.up * newMoverColliderHeight), out raycastHit, mover.sensorLayermask))
            {
                newMoverColliderHeight = raycastHit.distance - 0.01f;
            }

            mover.colliderHeight = Mathf.Max(newMoverColliderHeight, mover.colliderThickness);

            if (mover.colliderHeight >= stepHeightWorldUnits + mover.colliderThickness)
            {
                mover.stepHeight = stepHeightWorldUnits / mover.colliderHeight;
            }
            else
            {
                mover.stepHeight = Mathf.Max((mover.colliderHeight - stepHeightWorldUnits) /
                    mover.colliderHeight, 0f);
            }

            mover.RecalculateColliderDimensions();
        }

        private void GetInput()
        {
            if(!jumpPressed)
            {
                jumpPressed = PlayerController.Instance.CurrentVRRig.JumpButtonPressed;
            }
            
            if(!crouchPressed)
            {
                crouchPressed = PlayerController.Instance.CurrentVRRig.CrouchButtonPressed;
            }
        }

        private void ResetInput()
        {
            jumpPressed = false;
            crouchPressed = false;
        }

        private void HandleRotation()
        {
            var rotationInput = PlayerController.Instance.CurrentVRRig.RotationInput;

            if (rotationInput == 0)
            {
                return;
            }

            var deltaRotation = 0f;

            if(PlayerSettingsController.Instance.RotationMode == RotationModeEnum.Smooth)
            {
                deltaRotation = (rotationInput * (float)PlayerSettingsController.Instance.SmoothRotationSpeed * smoothRotationMultiplier * Time.fixedDeltaTime);
            }
            else
            {
                if(Time.time - snapRotationTime >= snapRotationCooldown)
                {
                    snapRotationTime = Time.time;

                    deltaRotation = rotationInput > 0 ? PlayerSettingsController.Instance.SnapRotationDegrees : -PlayerSettingsController.Instance.SnapRotationDegrees;
                }
            }

            if(deltaRotation != 0f)
            {
                if (PlayerSettingsController.Instance.RoomSetup == RoomSetupEnum.Roomscale)
                {
                    TeleportPlayerHead(Camera.main.transform.position, Camera.main.transform.eulerAngles.y + deltaRotation);
                }
                else
                {
                    TeleportPlayerHead(Camera.main.transform.position, PlayerController.Instance.transform.eulerAngles.y + deltaRotation);
                }
            }
        }

        private void HandleState()
        {
            var isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(momentum, mover.transform.up) > 0f);
            var isSliding = mover.IsGrounded() &&
                (Vector3.Angle(mover.GetGroundNormal(), mover.transform.up) > slopeLimit);

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

                    if ((Time.time - jumpStartTime) > jumpDuration)
                    {
                        currentMotorState = PlayerMotorStateEnum.Rising;
                        break;
                    }

                    //Check if jump key was let go;
                    //if (jumpKeyWasLetGo)
                    //    currentMotorState = PlayerMotorStateEnum.Rising;

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
                verticalMomentum -= mover.transform.up * slideGravity * Time.fixedDeltaTime;
            else
                verticalMomentum -= mover.transform.up * gravity * Time.fixedDeltaTime;

            if (currentMotorState == PlayerMotorStateEnum.Grounded)
                verticalMomentum = Vector3.zero;

            if (IsGrounded)
                horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(horizontalMomentum, groundFriction, Time.fixedDeltaTime, 0f);
            else
                horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(horizontalMomentum, airFriction, Time.fixedDeltaTime, 0f);

            momentum = horizontalMomentum + verticalMomentum;

            if (currentMotorState == PlayerMotorStateEnum.Sliding)
            {
                momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
            }

            if (currentMotorState == PlayerMotorStateEnum.Jumping)
            {
                momentum = VectorMath.RemoveDotVector(momentum, mover.transform.up);
                momentum += mover.transform.up * jumpSpeed;
            }
        }

        private void HandleJumping()
        {
            if (currentMotorState == PlayerMotorStateEnum.Grounded)
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
            if(crouching)
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

            velocity *= movementSpeed;

            //If controller is in the air, multiply movement velocity with 'airControl';
            if (!IsGrounded)
                velocity *= airControl;

            //If controller is standing (or walking) on a slope, decrease player velocity based on the slope's angle;
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
                yRotation = PlayerController.Instance.CurrentVRRig.MoveHand.eulerAngles.y;
            else
                yRotation = Camera.main.transform.eulerAngles.y;

            var movementInput = PlayerController.Instance.CurrentVRRig.MovementInput;

            return Quaternion.Euler(0f, yRotation, 0f) * new Vector3(movementInput.x, 0f, movementInput.y);
        }

        private bool IsRisingOrFalling()
        {
            var verticalMomentum = VectorMath.ExtractDotVector(momentum, mover.transform.up);

            return (verticalMomentum.magnitude > 0.001f);
        }

        private void GroundContactLost()
        {
            var horizontalMomentumSpeed = VectorMath.RemoveDotVector(momentum, mover.transform.up).magnitude;
            var currentVelocity = momentum + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(movementSpeed - horizontalMomentumSpeed, 0f, movementSpeed));

            var length = currentVelocity.magnitude;

            var velocityDirection = Vector3.zero;
            if (length != 0f)
            {
                velocityDirection = currentVelocity / length;
            }

            if (length >= movementSpeed * airControl)
            {
                length -= movementSpeed * airControl;
            }
            else
            {
                length = 0f;
            }

            momentum = velocityDirection * length;
        }

        private void GroundContactRegained(Vector3 collisionVelocity)
        {
            //Call 'OnLand' event;
            if (OnLand != null)
                OnLand(collisionVelocity);
        }

        private void JumpStart()
        {
            momentum += mover.transform.up * jumpSpeed;

            jumpStartTime = Time.time;

            if (OnJump != null)
            {
                OnJump(momentum);
            }
        }

        private void Crouch()
        {
            crouching = true;

            //PlayerController.Instance.CurrentVRRig.transform.localPosition = new Vector3(0f, -crouchDistance, 0f);
        }

        private void UnCrouch()
        {
            crouching = false;

            //PlayerController.Instance.CurrentVRRig.transform.localPosition = Vector3.zero;
        }

        #region Teleports
        public void TeleportPlayerRoom(Vector3 desiredWorldPositionOfRoom, Quaternion desiredWordRotationOfRoom)
        {
            PlayerController.Instance.transform.rotation = desiredWordRotationOfRoom;
            PlayerController.Instance.transform.position = desiredWorldPositionOfRoom;
        }

        public void TeleportPlayerHead(Vector3 desiredWorldPositionOfCamera)
        {
            TeleportPlayerRoom(desiredWorldPositionOfCamera + (PlayerController.Instance.transform.position - Camera.main.transform.position), PlayerController.Instance.transform.rotation);
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
                    (PlayerController.Instance.transform.position - Camera.main.transform.position)),
                    Quaternion.Euler(0f, lookAtYEulerAngle - Camera.main.transform.localEulerAngles.y, 0f));
            }
            else
            {
                TeleportPlayerRoom(desiredWorldPositionOfCamera + (Quaternion.Euler(0f, lookAtYEulerAngle - PlayerController.Instance.transform.eulerAngles.y, 0f) *
                    (PlayerController.Instance.transform.position - Camera.main.transform.position)),
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
