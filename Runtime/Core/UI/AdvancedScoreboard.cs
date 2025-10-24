using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.UI
{
    /// <summary>
    /// An advanced scoreboard that displays player names, kills, deaths, and ping when Mirror is present. When Mirror is absent,
    /// it displays local player info. Assign a vertical layout group with a prefab row containing Text elements.
    /// The prefab should have two Text children: Name (player name) and Stats (kills/deaths/ping). This component will
    /// instantiate a row for each player and update their stats periodically.
    /// </summary>
    public class AdvancedScoreboard : MonoBehaviour
    {
        [Tooltip("Container under which player rows will be instantiated (e.g., a VerticalLayoutGroup).")]
        public Transform rowsContainer;

        [Tooltip("Prefab representing a single player row (must have two Text components for name and stats).")]
        public GameObject rowPrefab;

        [Tooltip("Refresh interval in seconds.")]
        public float refreshInterval = 0.5f;

        private float nextUpdateTime;
        private readonly Dictionary<int, GameObject> rowInstances = new Dictionary<int, GameObject>();

        private void Update()
        {
            if (Time.time < nextUpdateTime) return;
            nextUpdateTime = Time.time + refreshInterval;
            UpdateScoreboard();
        }

        private void UpdateScoreboard()
        {
            #if HAS_MIRROR
            // Fetch players from Mirror connections
            List<NetworkConnection> connections = new List<NetworkConnection>();
            if (NetworkServer.active)
            {
                foreach (var kvp in NetworkServer.connections)
                {
                    if (kvp.Value != null && kvp.Value.identity != null)
                    {
                        connections.Add(kvp.Value);
                    }
                }
            }
            else if (NetworkClient.active)
            {
                // On client, we can only display our own info and maybe other player objects if spawned
                foreach (var kvp in NetworkClient.spawned)
                {
                    if (kvp.Value != null && kvp.Value.TryGetComponent(out NetworkIdentity id))
                    {
                        if (id.connectionToServer != null) connections.Add(id.connectionToServer);
                    }
                }
            }
            // Clear rows for missing connections
            foreach (var key in new List<int>(rowInstances.Keys))
            {
                if (!connections.Exists(c => c.connectionId == key))
                {
                    Destroy(rowInstances[key]);
                    rowInstances.Remove(key);
                }
            }
            // Update or create rows
            foreach (var conn in connections)
            {
                if (!rowInstances.TryGetValue(conn.connectionId, out var row))
                {
                    row = Instantiate(rowPrefab, rowsContainer);
                    rowInstances[conn.connectionId] = row;
                }
                var texts = row.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    // Name: use connectionId as name if no player data
                    string name = $"Player {conn.connectionId}";
                    // Stats: kills/deaths/ping (requires a hypothetical PlayerStats component with Mirror SyncVars)
                    int kills = 0;
                    int deaths = 0;
                    int ping = UMK.Core.UMK_NetworkService.Instance.Transport != null ? UMK.Core.UMK_NetworkService.Instance.Transport.GetPingMs() : 0;
                    // Try to get PlayerStats from connection's identity
                    if (conn.identity != null && conn.identity.TryGetComponent(out PlayerStats stats))
                    {
                        name = stats.playerName;
                        kills = stats.kills;
                        deaths = stats.deaths;
                        ping = stats.ping;
                    }
                    texts[0].text = name;
                    texts[1].text = $"{kills}/{deaths}  {ping}ms";
                }
            }
            #else
            // Offline: just display one row with local data
            if (rowsContainer == null || rowPrefab == null) return;
            if (rowInstances.Count == 0)
            {
                var row = Instantiate(rowPrefab, rowsContainer);
                rowInstances[0] = row;
            }
            var texts = rowInstances[0].GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = "LocalPlayer";
                texts[1].text = $"0/0 0ms";
            }
            #endif
        }
    }
    
    /// <summary>
    /// Example player stats component that could be attached to player objects. Uses Mirror SyncVars to replicate stats.
    /// This is optional; if absent, the scoreboard will fall back to connection IDs.
    /// </summary>
    #if HAS_MIRROR
    public class PlayerStats : NetworkBehaviour
    {
        [SyncVar]
        public string playerName = "Player";
        [SyncVar]
        public int kills;
        [SyncVar]
        public int deaths;
        [SyncVar]
        public int ping;
        private float pingTimer;
        public float pingUpdateInterval = 1f;
        void Update()
        {
            if (!isLocalPlayer) return;
            pingTimer += Time.deltaTime;
            if (pingTimer >= pingUpdateInterval)
            {
                pingTimer = 0f;
                ping = NetworkTime.rtt * 1000;
            }
        }
    }
    #endif
}