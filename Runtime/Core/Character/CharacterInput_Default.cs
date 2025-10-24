using UnityEngine;
using UnityEngine.InputSystem;

namespace UMK.Core.Character
{
    /// <summary>
    /// Default input source that sets up simple InputActions in code. This is used by CharacterAgent
    /// if no custom IInputSource is provided. Supports movement, look, jump, sprint, crouch, action and equip.
    /// </summary>
    public class CharacterInput_Default : IInputSource
    {
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _sprint;
        private InputAction _crouch;
        private InputAction _action;
        private InputAction _equip;

        public void CreateAndEnable()
        {
            _move = new InputAction("Move");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            _move.AddBinding("<Gamepad>/leftStick");
            _move.Enable();
            _look = new InputAction("Look");
            _look.AddBinding("<Mouse>/delta");
            _look.AddBinding("<Gamepad>/rightStick");
            _look.Enable();
            _jump = new InputAction("Jump", binding: "<Keyboard>/space");
            _jump.AddBinding("<Gamepad>/buttonSouth");
            _jump.Enable();
            _sprint = new InputAction("Sprint", binding: "<Keyboard>/leftShift");
            _sprint.AddBinding("<Gamepad>/leftStickPress");
            _sprint.Enable();
            _crouch = new InputAction("Crouch", binding: "<Keyboard>/leftCtrl");
            _crouch.AddBinding("<Gamepad>/rightStickPress");
            _crouch.Enable();
            _action = new InputAction("Action", binding: "<Keyboard>/e");
            _action.AddBinding("<Gamepad>/buttonWest");
            _action.Enable();
            _equip = new InputAction("Equip", binding: "<Keyboard>/f");
            _equip.AddBinding("<Gamepad>/buttonNorth");
            _equip.Enable();
        }
        public Vector2 GetMoveVector() => _move.ReadValue<Vector2>();
        public Vector2 GetLookVector() => _look.ReadValue<Vector2>();
        public bool GetJump() => _jump.triggered;
        public bool GetSprint() => _sprint.IsPressed();
        public bool GetCrouchToggle() => _crouch.triggered;
        public bool GetAction() => _action.triggered;
        public bool GetEquipToggle() => _equip.triggered;
    }
}