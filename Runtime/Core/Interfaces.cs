using System;
using UnityEngine;

namespace UMK.Core
{
    public interface ITransport
    {
        string Name { get; }
        bool Available { get; }
        void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError);
        void StartHost();
        void StartClient(string addressOrCode);
        void StartServer();
        int GetPingMs();
    }

    public interface IAntiCheatProvider
    {
        string Name { get; }
        bool Available { get; }
        void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError);
        void Enable();
        void Disable();
        event Action<string,int> OnViolation;
    }

    public interface IDiagnosticsProvider
    {
        void SetOverlayVisible(bool visible);
        void Toggle();
        void SetTransport(ITransport transport);
    }
}