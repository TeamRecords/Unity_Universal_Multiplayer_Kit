#if HAS_UTP && HAS_UGS_RELAY
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace UMK.Core
{
    public class UMK_UnityRelayTransport : ITransport
    {
        public string Name => "Unity Relay";
        public bool Available => true;
        UnityTransport _transport;
        NetworkManager _ngo;
        UMK_NetworkConfig _cfg;

        public void Initialize(GameObject context, UMK_NetworkConfig config, System.Action<string> onError)
        {
            _cfg = config;
            _transport = Object.FindFirstObjectByType<UnityTransport>();
            if (!_transport)
            {
                var go = new GameObject("UMK_UnityTransport");
                _transport = go.AddComponent<UnityTransport>();
            }
            _ngo = NetworkManager.Singleton;
            if (!_ngo)
            {
                onError?.Invoke("NGO NetworkManager not found. Add Unity.Netcode NetworkManager to use Relay.");
            }
        }

        public async void StartHost()
        {
            await EnsureServices();
            try
            {
                var alloc = await RelayService.Instance.CreateAllocationAsync(_cfg.maxRelayConnections);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
                var data = new RelayServerData(alloc, "dtls");
                _transport.SetRelayServerData(data);
                if (_ngo) _ngo.StartHost();
                Debug.Log($"[UMK] Relay Host started. JoinCode: {joinCode}");
            }
            catch (System.Exception e)
            {
                Debug.LogError("[UMK] Relay host failed: " + e.Message);
            }
        }

        public async void StartClient(string joinCode)
        {
            await EnsureServices();
            try
            {
                var joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var data = new RelayServerData(joinAlloc, "dtls");
                _transport.SetRelayServerData(data);
                if (_ngo) _ngo.StartClient();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[UMK] Relay client failed: " + e.Message);
            }
        }

        public void StartServer()
        {
            Debug.LogError("[UMK] Use Mirror KCP/Telepathy for dedicated servers, or NGO server without Relay.");
        }

        public int GetPingMs() => 0;

        static async Task EnsureServices()
        {
            if (Unity.Services.Core.UnityServices.State != ServicesInitializationState.Initialized)
                await Unity.Services.Core.UnityServices.InitializeAsync();
        }
    }
}
#else
using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_UnityRelayTransport : ITransport
    {
        public string Name => "Unity Relay (inactive)";
        public bool Available => false;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            onError?.Invoke("Unity Relay requires com.unity.transport + com.unity.services.relay + NGO packages.");
        }
        public void StartHost(){}
        public void StartClient(string code){}
        public void StartServer(){}
        public int GetPingMs() => 0;
    }
}
#endif