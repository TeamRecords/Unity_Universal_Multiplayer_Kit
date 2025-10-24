using UnityEngine;
using UnityEngine.UI;

namespace UMK.Core.UI
{
    /// <summary>
    /// Displays a simple scoreboard showing local ping and number of connected players when Mirror is present.
    /// Requires two Text components to assign for ping and players count. You can extend this to display
    /// player names and scores by listening to EventBus or Mirror callbacks.
    /// </summary>
    public class NetworkScoreboard : MonoBehaviour
    {
        public Text pingLabel;
        public Text playersLabel;
        public float refreshInterval = 0.5f;
        private float _nextTime;
        void Update()
        {
            if (Time.time < _nextTime) return;
            _nextTime = Time.time + refreshInterval;
            UpdatePing();
            UpdatePlayers();
        }
        private void UpdatePing()
        {
            if (pingLabel == null) return;
            var svc = UMK.Core.UMK_NetworkService.Instance;
            if (svc != null && svc.Transport != null)
            {
                int ping = svc.Transport.GetPingMs();
                pingLabel.text = $"Ping: {ping} ms";
            }
            else
            {
                pingLabel.text = "Ping: N/A";
            }
        }
        private void UpdatePlayers()
        {
            if (playersLabel == null) return;
#if HAS_MIRROR
            int count = 0;
            if (Mirror.NetworkServer.active)
            {
                count = Mirror.NetworkServer.connections.Count;
            }
            else if (Mirror.NetworkClient.active)
            {
                // On client we don't have full server connections list; approximate by counting spawned players
                count = Mirror.NetworkClient.readyConnections.Count;
            }
            playersLabel.text = $"Players: {count}";
#else
            playersLabel.text = "Players: 1";
#endif
        }
    }
}