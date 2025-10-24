#if HAS_MIRROR
using System;
using UnityEngine;
using Mirror;
using kcp2k;

namespace UMK.Core
{
    public class UMK_MirrorKcpTransport : ITransport
    {
        public string Name => "Mirror KCP";
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
            if (!UnityEngine.Object.FindFirstObjectByType<KcpTransport>())
                _nm.gameObject.AddComponent<KcpTransport>();
        }
        public void StartHost(){ _nm.StartHost(); }
        public void StartClient(string address){ _nm.networkAddress = address; _nm.StartClient(); }
        public void StartServer(){ _nm.StartServer(); }
        public int GetPingMs() => (int)(NetworkTime.rtt*1000.0);
    }
}
#endif