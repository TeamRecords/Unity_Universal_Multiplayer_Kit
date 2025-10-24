using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_OfflineTransport : ITransport
    {
        public string Name => "Offline";
        public bool Available => true;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError){}
        public void StartHost(){ Debug.Log("[UMK] Host (offline)"); }
        public void StartClient(string address){ Debug.Log("[UMK] Client (offline)"); }
        public void StartServer(){ Debug.Log("[UMK] Server (offline)"); }
        public int GetPingMs() => 0;
    }
}