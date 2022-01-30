using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace _Project.Scripts
{
    [RequireComponent(typeof(InputController))]
    public class PlayerController : MonoBehaviour
    {
        readonly Vector3 flippedScale = new Vector3(-1, 1, 1);
        readonly Vector3 flippedScale_shadow = new Vector3(-1, -1, 1);
        readonly Quaternion flippedRotation = new Quaternion(0, 0, 1, 0);

        [Header("Character")]
        [SerializeField] Animator animator = null;
        [SerializeField] Animator animator_shadow = null;
        [SerializeField] Transform puppet = null;
        [SerializeField] Transform puppet_shadow = null;
        [SerializeField] CharacterAudio audioPlayer = null;
        [SerializeField] private PlayableDirector FlipFX;

        [Header("Tail")]
        [SerializeField] Transform tailAnchor = null;
        [SerializeField] Rigidbody2D tailRigidbody = null;

        [Header("Equipment")]
        [SerializeField] Transform handAnchor = null;
        [SerializeField] SpriteLibrary spriteLibrary = null;

        [Header("Movement")]
        [SerializeField] float acceleration = 0.0f;
        [SerializeField] float maxSpeed = 0.0f;
        [SerializeField] float jumpForce = 0.0f;
        [SerializeField] float minFlipSpeed = 0.1f;
        [SerializeField] float jumpGravityScale = 1.0f;
        [SerializeField] float fallGravityScale = 1.0f;
        [SerializeField] float groundedGravityScale = 1.0f;
        [SerializeField] bool resetSpeedOnLand = false;

        private Rigidbody2D controllerRigidbody;
        private Collider2D controllerCollider;
        private InputController controllerInput;
        private LayerMask softGroundMask;
        private LayerMask hardGroundMask;

        private Vector2 movementInput;
        private bool jumpInput;
        private bool flipInput;

        private Vector2 prevVelocity;
        private GroundType groundType;
        public bool isFlipped;
        private bool isJumping;
        private bool isFalling;

        private int animatorGroundedBool;
        private int animatorRunningSpeed;
        private int animatorJumpTrigger;

        public bool CanMove { get; set; }

        void Start()
        {
#if UNITY_EDITOR
            if (Keyboard.current == null)
            {
                var playerSettings = new UnityEditor.SerializedObject(Resources.FindObjectsOfTypeAll<UnityEditor.PlayerSettings>()[0]);
                var newInputSystemProperty = playerSettings.FindProperty("enableNativePlatformBackendsForNewInputSystem");
                bool newInputSystemEnabled = newInputSystemProperty != null ? newInputSystemProperty.boolValue : false;

                if (newInputSystemEnabled)
                {
                    var msg = "New Input System backend is enabled but it requires you to restart Unity, otherwise the player controls won't work. Do you want to restart now?";
                    if (UnityEditor.EditorUtility.DisplayDialog("Warning", msg, "Yes", "No"))
                    {
                        UnityEditor.EditorApplication.ExitPlaymode();
                        var dataPath = Application.dataPath;
                        var projectPath = dataPath.Substring(0, dataPath.Length - 7);
                        UnityEditor.EditorApplication.OpenProject(projectPath);
                    }
                }
            }
#endif

            controllerRigidbody = GetComponent<Rigidbody2D>();
            controllerCollider = GetComponent<Collider2D>();
            controllerInput = GetComponent<InputController>();
            softGroundMask = LayerMask.GetMask("Ground Soft");
            hardGroundMask = LayerMask.GetMask("Ground Hard");

            animatorGroundedBool = Animator.StringToHash("Grounded");
            animatorRunningSpeed = Animator.StringToHash("RunningSpeed");
            animatorJumpTrigger = Animator.StringToHash("Jump");

            CanMove = true;
        }

        private void Update()
        {
            if (!CanMove) return;

            // Horizontal movement
            float moveHorizontal = controllerInput.Move.x;
            
            movementInput = new Vector2(moveHorizontal, 0);
            
            // Jumping input
            if (!isJumping && controllerInput.Jump)
                jumpInput = true;
            
            // World Flip input
            if (controllerInput.FlipWorld)
                flipInput = true;
        }

        void FixedUpdate()
        {
            UpdateGrounding();
            UpdateVelocity();
            UpdateDirection();
            UpdateJump();
            UpdateTailPose();
            UpdateGravityScale();
            UpdateWorld();

            prevVelocity = controllerRigidbody.velocity;
        }
        
        private void UpdateWorld()
        {
            if (flipInput)
            {
                FlipFX.Play();
                //WorldFlipManager.Instance.FlipWorld();

                flipInput = false;
            }
        }

        private void UpdateGrounding()
        {
            // Use character collider to check if touching ground layers
            if (controllerCollider.IsTouchingLayers(softGroundMask))
                groundType = GroundType.Soft;
            else if (controllerCollider.IsTouchingLayers(hardGroundMask))
                groundType = GroundType.Hard;
            else
                groundType = GroundType.None;

            // Update animator
            animator.SetBool(animatorGroundedBool, groundType != GroundType.None);
            animator_shadow.SetBool(animatorGroundedBool, groundType != GroundType.None);
        }

        private void UpdateVelocity()
        {
            Vector2 velocity = controllerRigidbody.velocity;

            // Apply acceleration directly as we'll want to clamp
            // prior to assigning back to the body.
            velocity += movementInput * acceleration * Time.fixedDeltaTime;

            // We've consumed the movement, reset it.
            movementInput = Vector2.zero;

            // Clamp horizontal speed.
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);

            // Assign back to the body.
            controllerRigidbody.velocity = velocity;

            // Update animator running speed
            var horizontalSpeedNormalized = Mathf.Abs(velocity.x) / maxSpeed;
            animator.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);
            animator_shadow.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);

            // Play audio
            audioPlayer.PlaySteps(groundType, horizontalSpeedNormalized);
        }

        private void UpdateJump()
        {
            // Set falling flag
            if (isJumping && controllerRigidbody.velocity.y < 0)
                isFalling = true;

            // Jump
            if (jumpInput && groundType != GroundType.None)
            {
                // Jump using impulse force
                controllerRigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                // Set animator
                animator.SetTrigger(animatorJumpTrigger);
                animator_shadow.SetTrigger(animatorJumpTrigger);

                // We've consumed the jump, reset it.
                jumpInput = false;

                // Set jumping flag
                isJumping = true;

                // Play audio
                audioPlayer.PlayJump();
            }

            // Landed
            else if (isJumping && isFalling && groundType != GroundType.None)
            {
                // Since collision with ground stops rigidbody, reset velocity
                if (resetSpeedOnLand)
                {
                    prevVelocity.y = controllerRigidbody.velocity.y;
                    controllerRigidbody.velocity = prevVelocity;
                }

                // Reset jumping flags
                isJumping = false;
                isFalling = false;

                // Play audio
                audioPlayer.PlayLanding(groundType);
            }
        }

        private void UpdateDirection()
        {
            // Use scale to flip character depending on direction
            if (controllerRigidbody.velocity.x > minFlipSpeed && isFlipped)
            {
                FlipDirection(false);
            }
            else if (controllerRigidbody.velocity.x < -minFlipSpeed && !isFlipped)
            {
                FlipDirection(true);
            }
        }

        private void UpdateTailPose()
        {
            // Calculate the extrapolated target position of the tail anchor.
            Vector2 targetPosition = tailAnchor.position;
            targetPosition += controllerRigidbody.velocity * Time.fixedDeltaTime;

            tailRigidbody.MovePosition(targetPosition);
            if (isFlipped)
                tailRigidbody.SetRotation(tailAnchor.rotation * flippedRotation);
            else
                tailRigidbody.SetRotation(tailAnchor.rotation);
        }

        private void UpdateGravityScale()
        {
            // Use grounded gravity scale by default.
            var gravityScale = groundedGravityScale;

            if (groundType == GroundType.None)
            {
                // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
                gravityScale = controllerRigidbody.velocity.y > 0.0f ? jumpGravityScale : fallGravityScale;           
            }

            controllerRigidbody.gravityScale = gravityScale;
        }

        public void FlipDirection(bool flip)
        {
            isFlipped = flip;
            puppet.localScale = flip ? flippedScale : Vector3.one;
            puppet_shadow.localScale = flip ? flippedScale_shadow : new Vector3(1, -1, 1);
        }

        public void GrabItem(Transform item)
        {
            // Attach item to hand
            item.SetParent(handAnchor, false);
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
        }

        public void SwapSprites(SpriteLibraryAsset spriteLibraryAsset)
        {
            spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
        }
    }
}