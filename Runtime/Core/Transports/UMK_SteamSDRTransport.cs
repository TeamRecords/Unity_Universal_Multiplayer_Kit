#if HAS_MIRROR
using System;
using System.Linq;
using UnityEngine;
using Mirror;

namespace UMK.Core
{
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

            string[] candidate = { "SteamSocketsTransport", "FizzySteamworks", "FacepunchTransport" };
            foreach (var name in candidate)
            {
                var t = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
                    .FirstOrDefault(x => x.Name == name);
                if (t != null)
                {
                    _steamTransport = _nm.gameObject.GetComponent(t) ?? _nm.gameObject.AddComponent(t);
                    break;
                }
            }
            if (_steamTransport == null)
            {
                onError?.Invoke("Steam transport not found. Add SteamSocketsTransport or FizzySteamworks to NetworkManager.");
            }
        }

        public void StartHost(){ _nm.StartHost(); }
        public void StartClient(string _){ _nm.StartClient(); } // address not used for Steam lobby join here
        public void StartServer(){ _nm.StartServer(); }
        public int GetPingMs() => (int)(NetworkTime.rtt*1000.0);
    }
}
#endif