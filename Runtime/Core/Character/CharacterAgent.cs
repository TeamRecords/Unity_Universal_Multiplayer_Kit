using UnityEngine;
using UnityEngine.InputSystem;

namespace UMK.Core.Character
{
    /// <summary>
    /// High-level orchestrator for a player controlled or AI-driven character. It ties together
    /// input, movement, looking and equipment. In local play, it polls an IInputSource implementation
    /// and drives the CharacterMotor and CharacterLook accordingly. In networked play, only the local
    /// player executes input logic; remote players simply respond to replicated state.
    /// </summary>
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(CharacterLook))]
    [RequireComponent(typeof(CharacterEquipment))]
    public class CharacterAgent : NetBehaviourBase
    {
        public CharacterMotor motor;
        public CharacterLook look;
        public CharacterEquipment equipment;
        public IInputSource inputSource;

        // Camera pivot used by CharacterLook to apply pitch
        [Header("Camera Setup")]
        public Transform cameraRoot;
        public Camera playerCamera;

        void Awake()
        {
            if (!motor) motor = GetComponent<CharacterMotor>();
            if (!look) look = GetComponent<CharacterLook>();
            if (!equipment) equipment = GetComponent<CharacterEquipment>();
            // Assign camera to look system
            look.cameraRoot = cameraRoot;
            look.playerCamera = playerCamera;
        }

        void Start()
        {
            if (isLocalPlayer)
            {
                // Default input source if none provided
                if (inputSource == null) inputSource = new CharacterInput_Default();
                if (inputSource is CharacterInput_Default def)
                    def.CreateAndEnable();
                // Ensure camera/audio listener only for local
                if (playerCamera) playerCamera.enabled = true;
            }
            else
            {
                // Disable camera and audio listener for remote characters
                if (playerCamera) playerCamera.enabled = false;
                var audio = playerCamera != null ? playerCamera.GetComponent<AudioListener>() : null;
                if (audio) audio.enabled = false;
            }
        }

        void Update()
        {
            if (!isLocalPlayer) return;
            if (inputSource == null) return;
            // Movement
            Vector2 move = inputSource.GetMoveVector();
            bool sprint = inputSource.GetSprint();
            bool jump = inputSource.GetJump();
            bool crouchToggle = inputSource.GetCrouchToggle();
            motor.Tick(move, sprint, jump, crouchToggle);
            // Look
            Vector2 lookInput = inputSource.GetLookVector();
            look.Tick(lookInput);
            // Action / Interact
            if (inputSource.GetAction())
            {
                // Raycast to interactables
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 3f))
                {
                    var interactable = hit.collider.GetComponentInParent<IInteractable>();
                    interactable?.Interact(this);
                }
            }
            // Equip toggle
            if (inputSource.GetEquipToggle())
            {
                equipment.ToggleEquip();
            }
            // Tick equipment
            equipment.Tick(this);
        }
    }
}