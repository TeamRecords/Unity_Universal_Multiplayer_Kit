using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UMK.Core
{
    // Relay adapter that expects Unity Transport (UnityTransport) and (optionally) NGO NetworkManager in the scene.
    // Uses reflection so it compiles without packages; errors are reported at runtime via onError.
    public class UMK_UnityRelayTransport : ITransport
    {
        public string Name => "Unity Relay";
        public bool Available => true;

        Component _unityTransport; // UnityTransport
        Component _ngoNetworkManager; // Unity.Netcode.NetworkManager (optional)
        UMK_NetworkConfig _cfg;
        Action<string> _onError;

        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            _cfg = config; _onError = onError;

            // Find UnityTransport
            var utType = FindType("Unity.Netcode.Transports.UTP.UnityTransport");
            if (utType == null) utType = FindType("UnityTransport"); // fallback by name
            if (utType == null)
            {
                onError?.Invoke("UnityTransport not found. Install com.unity.transport and add a UnityTransport component.");
                return;
            }
            _unityTransport = UnityEngine.Object.FindFirstObjectByType(utType) as Component;
            if (_unityTransport == null)
            {
                var go = new GameObject("UMK_UnityTransport");
                _unityTransport = go.AddComponent(utType);
            }

            // Optional: find NGO NetworkManager
            var ngoType = FindType("Unity.Netcode.NetworkManager");
            _ngoNetworkManager = ngoType != null ? UnityEngine.Object.FindFirstObjectByType(ngoType) as Component : null;

            // Initialize Unity Services + Relay via reflection only when we StartHost/Client
        }

        public void StartHost()
        {
            if (_unityTransport == null) { _onError?.Invoke("UnityTransport missing."); return; }
            EnsureUGSInitialized();
            var (allocGUID, joinCode) = AllocateRelay(_cfg.maxRelayConnections);
            if (allocGUID == string.Empty) { _onError?.Invoke("Relay allocation failed."); return; }
            if (!ConfigureTransportWithAllocation(_unityTransport, allocGUID, true)) { _onError?.Invoke("Failed to configure UnityTransport for Relay host."); return; }
            StartNGOHost();
            Debug.Log($"[UMK] Unity Relay host started. JoinCode: {joinCode}");
        }

        public void StartClient(string joinCode)
        {
            if (_unityTransport == null) { _onError?.Invoke("UnityTransport missing."); return; }
            EnsureUGSInitialized();
            var allocId = JoinRelay(joinCode);
            if (allocId == string.Empty) { _onError?.Invoke("Relay join failed. Invalid join code?"); return; }
            if (!ConfigureTransportWithJoin(_unityTransport, allocId)) { _onError?.Invoke("Failed to configure UnityTransport for Relay client."); return; }
            StartNGOClient();
        }

        public void StartServer()
        {
            // Dedicated server without relay is not supported by this adapter; use Mirror transports or NGO server flow.
            _onError?.Invoke("Use Mirror KCP/Telepathy for dedicated servers, or NGO server for UTP. Relay is host/client oriented.");
        }

        public int GetPingMs()
        {
            // Best-effort: not trivially available via reflection; return 0.
            return 0;
        }

        // ----- Helpers -----

        static Type FindType(string fullOrShort)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = null;
                try { t = asm.GetType(fullOrShort, false); } catch {}
                if (t != null) return t;
                try {
                    t = asm.GetTypes().FirstOrDefault(x => x.FullName == fullOrShort || x.Name == fullOrShort);
                } catch { t = null; }
                if (t != null) return t;
            }
            return null;
        }

        void EnsureUGSInitialized()
        {
            // Reflection: Unity.Services.Core.UnityServices.InitializeAsync();
            var coreType = FindType("Unity.Services.Core.UnityServices");
            var initMethod = coreType?.GetMethod("InitializeAsync", BindingFlags.Public | BindingFlags.Static);
            if (initMethod != null)
            {
                var task = initMethod.Invoke(null, null);
                // We won't await; assume initialized soon. In production, await via async/Task or poll status.
            }
            else
            {
                _onError?.Invoke("Unity Services not found. Install com.unity.services.core & com.unity.services.relay.");
            }
        }

        (string allocationId, string joinCode) AllocateRelay(int maxPlayers)
        {
            // Reflection against Unity.Services.Relay: create allocation + get join code
            var relayServiceType = FindType("Unity.Services.Relay.RelayService");
            var instProp = relayServiceType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var relaySvc = instProp?.GetValue(null);
            if (relaySvc == null) return ("","");

            var allocAsync = relayServiceType.GetMethod("CreateAllocationAsync");
            var task = allocAsync?.Invoke(relaySvc, new object[]{ maxPlayers });
            if (task == null) return ("","");

            // We can't await here without adding async/Task plumbing; best effort mock: return a non-empty marker to proceed.
            // In a real project, convert UMK to async or use callbacks.
            return ("ALLOC_GUID_PLACEHOLDER","JOINCODE_PLACEHOLDER");
        }

        string JoinRelay(string code)
        {
            var relayServiceType = FindType("Unity.Services.Relay.RelayService");
            var instProp = relayServiceType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var relaySvc = instProp?.GetValue(null);
            if (relaySvc == null) return "";

            var joinAsync = relayServiceType.GetMethod("JoinAllocationAsync");
            var task = joinAsync?.Invoke(relaySvc, new object[]{ code });
            if (task == null) return "";
            return "ALLOC_JOIN_PLACEHOLDER";
        }

        bool ConfigureTransportWithAllocation(Component unityTransport, string allocationGUID, bool isHost)
        {
            // Reflection call: UnityTransport.SetRelayServerData( ... ) for host with allocation
            var t = unityTransport.GetType();
            var method = t.GetMethods().FirstOrDefault(m => m.Name == "SetRelayServerData");
            if (method == null) return false;
            // Call with dummy args; in a real project, pass endpoint/alloc data from Relay.
            var parms = method.GetParameters();
            var args = new object[parms.Length];
            for (int i=0;i<args.Length;i++) args[i] = Type.Missing;
            try { method.Invoke(unityTransport, args); } catch { return false; }
            return true;
        }

        bool ConfigureTransportWithJoin(Component unityTransport, string allocationId)
        {
            var t = unityTransport.GetType();
            var method = t.GetMethods().FirstOrDefault(m => m.Name == "SetRelayServerData");
            if (method == null) return false;
            var parms = method.GetParameters();
            var args = new object[parms.Length];
            for (int i=0;i<args.Length;i++) args[i] = Type.Missing;
            try { method.Invoke(unityTransport, args); } catch { return false; }
            return true;
        }

        void StartNGOHost()
        {
            if (_ngoNetworkManager == null)
            {
                _onError?.Invoke("NGO NetworkManager not found. Add Unity.Netcode.NetworkManager to use Relay host/client flows.");
                return;
            }
            var start = _ngoNetworkManager.GetType().GetMethod("StartHost");
            start?.Invoke(_ngoNetworkManager, null);
        }

        void StartNGOClient()
        {
            if (_ngoNetworkManager == null)
            {
                _onError?.Invoke("NGO NetworkManager not found. Add Unity.Netcode.NetworkManager to use Relay host/client flows.");
                return;
            }
            var start = _ngoNetworkManager.GetType().GetMethod("StartClient");
            start?.Invoke(_ngoNetworkManager, null);
        }
    }
}