using UnityEngine;
using UnityEngine.InputSystem;

namespace UMK.Core.Character
{
    /// <summary>
    /// Handles first person camera rotation. Applies yaw to the character and pitch to a camera root
    /// (pivot) to allow independent clamping. Use in conjunction with CharacterAgent and CharacterInput.
    /// </summary>
    public class CharacterLook : MonoBehaviour
    {
        [Header("References")]
        public Transform cameraRoot;
        public Camera playerCamera;

        [Header("Sensitivity")]
        public float mouseSensitivity = 0.12f;
        public float gamepadSensitivity = 120f;
        [Header("Pitch Limits")]
        public float minPitch = -85f;
        public float maxPitch = 80f;
        private float _pitch;

        /// <summary>
        /// Performs one frame of look rotation based on input vector (x=mouseX, y=mouseY). This
        /// should be called from CharacterAgent.Update for the local player.
        /// </summary>
        public void Tick(Vector2 lookInput)
        {
            // Determine whether to use mouse or gamepad sensitivity. Heuristic: if a gamepad is present
            // and the right stick has non-zero magnitude, treat as gamepad input.
            bool isGamepad = Gamepad.current != null && Gamepad.current.rightStick.ReadValue() != Vector2.zero;
            if (isGamepad)
            {
                float yaw = lookInput.x * gamepadSensitivity * Time.deltaTime;
                float dpitch = -lookInput.y * gamepadSensitivity * Time.deltaTime;
                transform.Rotate(0f, yaw, 0f);
                _pitch = Mathf.Clamp(_pitch + dpitch, minPitch, maxPitch);
            }
            else
            {
                float yaw = lookInput.x * mouseSensitivity;
                float dpitch = -lookInput.y * mouseSensitivity;
                transform.Rotate(0f, yaw, 0f);
                _pitch = Mathf.Clamp(_pitch + dpitch, minPitch, maxPitch);
            }
            if (cameraRoot) cameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}