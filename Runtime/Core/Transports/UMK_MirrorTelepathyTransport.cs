#if HAS_MIRROR
using System;
using UnityEngine;
using Mirror;

namespace UMK.Core
{
    public class UMK_MirrorTelepathyTransport : ITransport
    {
        public string Name => "Mirror Telepathy";
        public bool Available => true;
        NetworkManager _nm;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            _nm = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
            if (!_nm)
            {
                var go = new GameObject("UMK_NetworkManager");
                _nm = go.AddComponent<NetworkManager>();
            }
            if (!UnityEngine.Object.FindFirstObjectByType<TelepathyTransport>())
                _nm.gameObject.AddComponent<TelepathyTransport>();
        }
        public void StartHost(){ _nm.StartHost(); }
        public void StartClient(string address){ _nm.networkAddress = address; _nm.StartClient(); }
        public void StartServer(){ _nm.StartServer(); }
        public int GetPingMs() => (int)(NetworkTime.rtt*1000.0);
    }
}
#endif