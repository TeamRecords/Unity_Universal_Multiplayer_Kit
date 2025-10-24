using UnityEngine;
using System.Collections.Generic;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Puzzle
{
    /// <summary>
    /// Aggregates multiple puzzle components and tracks completion. When all puzzles are solved, fires an event.
    /// </summary>
    public class PuzzleManager : NetBehaviourBase
    {
        [Tooltip("List of puzzle components managed by this manager.")]
        public List<MonoBehaviour> puzzles = new List<MonoBehaviour>();

        [Tooltip("Whether all puzzles have been solved.")]
        #if HAS_MIRROR
        [SyncVar(hook = nameof(OnAllSolvedChanged))]
        #endif
        private bool allSolved;

        private void Update()
        {
            if (IsServerOrOffline() && !allSolved)
            {
                CheckPuzzles();
            }
        }

        private void CheckPuzzles()
        {
            foreach (var puzzle in puzzles)
            {
                if (puzzle == null) continue;
                bool solved = false;
                if (puzzle is KeypadPuzzle kp)
                {
                    solved = (bool)kp.GetType().GetField("isSolved", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(kp);
                }
                else if (puzzle is LeverSwitch lever)
                {
                    solved = lever.isOn;
                }
                else
                {
                    // Unknown puzzle type
                    continue;
                }
                if (!solved)
                {
                    return; // at least one unsolved
                }
            }
            allSolved = true;
            OnAllSolvedChanged(false, true);
        }

        private void OnAllSolvedChanged(bool oldVal, bool newVal)
        {
            if (newVal)
            {
                Debug.Log("All puzzles solved!");
                // Fire an event or call logic here
            }
        }
    }
}