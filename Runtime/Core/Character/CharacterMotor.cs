using System;
using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Handles character locomotion using Unity's CharacterController. Supports walking, sprinting,
    /// crouching, jumping and gravity. Emits footstep events via EventBus for audio/AI systems.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : MonoBehaviour
    {
        [Header("Movement Speeds")]
        public float walkSpeed = 3.0f;
        public float sprintSpeed = 5.0f;
        public float crouchSpeed = 1.8f;
        public float jumpHeight = 1.0f;
        [Header("Crouch")]
        public float standHeight = 1.8f;
        public float crouchHeight = 1.3f;
        public float heightLerpSpeed = 12f;
        [Header("Gravity")]
        public float gravity = -20f;
        [Header("Footsteps")]
        public float stepDistanceWalk = 1.8f;
        public float stepDistanceSprint = 1.3f;
        public float stepDistanceCrouch = 2.2f;

        private CharacterController _cc;
        private Vector3 _velocity;
        private bool _isCrouching;
        private float _stepMeter;
        private float _currentSpeed;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Executes one frame of movement based on input. Should be called from CharacterAgent.Update
        /// on the local player.
        /// </summary>
        public void Tick(Vector2 moveInput, bool sprint, bool jump, bool crouchToggle)
        {
            if (crouchToggle) _isCrouching = !_isCrouching;

            // Adjust height smoothly
            float targetHeight = _isCrouching ? crouchHeight : standHeight;
            _cc.height = Mathf.Lerp(_cc.height, targetHeight, Time.deltaTime * heightLerpSpeed);
            _cc.center = new Vector3(0, _cc.height * 0.5f, 0);

            // Determine speed
            _currentSpeed = _isCrouching ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);
            // Transform input into world space
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 worldDir = transform.TransformDirection(inputDir);
            Vector3 planarVel = worldDir * _currentSpeed;

            // Jump & gravity
            if (_cc.isGrounded)
            {
                _velocity.y = -1f; // keep grounded
                if (jump && jumpHeight > 0f && !_isCrouching)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            Vector3 finalVelocity = planarVel + new Vector3(0, _velocity.y, 0);
            _cc.Move(finalVelocity * Time.deltaTime);

            // Footstep handling
            HandleFootsteps(planarVel);
        }

        private void HandleFootsteps(Vector3 planarVel)
        {
            float speed = new Vector3(planarVel.x, 0, planarVel.z).magnitude;
            if (speed < 0.1f || !_cc.isGrounded) return;
            float dist = _isCrouching ? stepDistanceCrouch : (Mathf.Approximately(_currentSpeed, sprintSpeed) ? stepDistanceSprint : stepDistanceWalk);
            _stepMeter += speed * Time.deltaTime;
            if (_stepMeter >= dist)
            {
                _stepMeter = 0f;
                // Publish a footstep event. Listeners can play audio or notify AI.
                EventBus.Publish(new FootstepEvent { position = transform.position, speed = speed });
            }
        }

        /// <summary>
        /// Simple data structure for footstep events. You can extend this to include surface type etc.
        /// </summary>
        public struct FootstepEvent
        {
            public Vector3 position;
            public float speed;
        }
    }
}