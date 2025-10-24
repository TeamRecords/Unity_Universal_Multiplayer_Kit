#if HAS_MIRROR
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Mirror;

namespace UMK.Core
{
    // Looks for a Steam-capable Mirror transport (e.g., "SteamSocketsTransport", "FizzySteamworks").
    public class UMK_SteamSDRTransport : ITransport
    {
        public string Name => "Steam (SDR)";
        public bool Available => true;
        NetworkManager _nm;
        Component _steamTransport;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            _nm = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
            if (!_nm)
            {
                var go = new GameObject("UMK_NetworkManager");
                _nm = go.AddComponent<NetworkManager>();
            }

            // Find an attached Steam transport by known type names
            string[] candidateTypes = new[] {
                "SteamSocketsTransport",          // Facepunch or community
                "FizzySteamworks",                // classic Mirror transport
                "FacepunchTransport",             // fallback name used by some forks
            };

            foreach (var tname in candidateTypes)
            {
                var t = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
                    .FirstOrDefault(x => x.Name == tname);
                if (t != null)
                {
                    _steamTransport = _nm.gameObject.GetComponent(t) ?? _nm.gameObject.AddComponent(t);
                    break;
                }
            }

            if (_steamTransport == null)
            {
                onError?.Invoke("No Steam transport found. Please add a Steam Sockets transport (e.g., SteamSocketsTransport/FizzySteamworks) to NetworkManager.");
            }
        }

        public void StartHost(){ _nm.StartHost(); }
        public void StartClient(string address){ _nm.StartClient(); } // Steam uses lobby/matchmaking; address is typically unused
        public void StartServer(){ _nm.StartServer(); }
        public int GetPingMs() => (int)(NetworkTime.rtt*1000.0);
    }
}
#endif