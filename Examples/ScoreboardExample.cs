using UnityEngine;
using UMK.Core.UI;

/// <summary>
/// Demonstrates how to set up an AdvancedScoreboard. Assign the rows container (with a VerticalLayoutGroup)
/// and row prefab (two Text components) via inspector. You can optionally assign custom refresh intervals.
/// </summary>
public class ScoreboardExample : MonoBehaviour
{
    public AdvancedScoreboard scoreboard;
    public Transform rowsContainer;
    public GameObject rowPrefab;

    void Awake()
    {
        if (scoreboard != null)
        {
            scoreboard.rowsContainer = rowsContainer;
            scoreboard.rowPrefab = rowPrefab;
            scoreboard.refreshInterval = 0.5f;
        }
    }
}