using UnityEngine;
using System.Text;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Puzzle
{
    /// <summary>
    /// A simple keypad puzzle with a predefined unlock code. Players can input digits via the keypad; when the code is correct,
    /// the puzzle is solved. The current input and solved state are replicated across the network when Mirror is present.
    /// </summary>
    public class KeypadPuzzle : NetBehaviourBase, IInteractable
    {
        [Tooltip("Correct code to unlock the puzzle.")]
        public string unlockCode = "1234";

        [Tooltip("Current input (replicated).")]
        #if HAS_MIRROR
        [SyncVar]
        #endif
        private string currentInput = string.Empty;

        [Tooltip("Whether the puzzle has been solved.")]
        #if HAS_MIRROR
        [SyncVar(hook = nameof(OnSolvedChanged))]
        #endif
        private bool isSolved;

        [Tooltip("Reset input on wrong code.")]
        public bool resetOnWrong = true;

        /// <summary>
        /// Add a digit (0-9) to the current input. If input length equals unlock code length, checks the code.
        /// </summary>
        public void AddDigit(int digit)
        {
            if (IsServerOrOffline() && !isSolved)
            {
                currentInput += digit.ToString();
                if (currentInput.Length >= unlockCode.Length)
                {
                    CheckCode();
                }
            }
        }

        private void CheckCode()
        {
            if (currentInput == unlockCode)
            {
                isSolved = true;
                OnSolvedChanged(false, true);
            }
            else if (resetOnWrong)
            {
                currentInput = string.Empty;
            }
            else
            {
                // remove oldest digit
                currentInput = currentInput.Substring(1);
            }
        }

        public void Interact(NetBehaviourBase interactor)
        {
            // Example: digits can be input via Interact call if keyed by 0-9; this is just a placeholder
            // In a real game, call AddDigit from UI or input system.
            if (interactor != null)
            {
                Debug.Log("KeypadPuzzle was interacted with by " + interactor.gameObject.name);
            }
        }

        private void OnSolvedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                Debug.Log("KeypadPuzzle solved!");
                // Additional solved logic here
            }
        }
    }
}