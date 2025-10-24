using UnityEngine;
using UMK.Core.Puzzle;

/// <summary>
/// Demonstrates how to set up a simple puzzle consisting of a lever and keypad. When both are completed,
/// the PuzzleManager logs a message. Assign the LeverSwitch, KeypadPuzzle and PuzzleManager components via inspector.
/// </summary>
public class PuzzleExample : MonoBehaviour
{
    public LeverSwitch lever;
    public KeypadPuzzle keypad;
    public PuzzleManager manager;

    void Start()
    {
        if (manager != null)
        {
            manager.puzzles.Clear();
            if (lever != null) manager.puzzles.Add(lever);
            if (keypad != null) manager.puzzles.Add(keypad);
        }
    }
}