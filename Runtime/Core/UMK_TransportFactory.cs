using System;

namespace UMK.Core
{
    public static class UMK_TransportFactory
    {
        public static ITransport Create(UMK_NetworkConfig cfg, System.Action<string> onError)
        {
#if HAS_MIRROR
            switch (cfg.transport)
            {
                case TransportKind.Telepathy: return new UMK_MirrorTelepathyTransport();
                case TransportKind.Kcp: return new UMK_MirrorKcpTransport();
                case TransportKind.SteamSDR: return new UMK_SteamSDRTransport();
                case TransportKind.UnityRelay: return new UMK_UnityRelayTransport();
                default: return new UMK_MirrorAutoTransport();
            }
#else
            if (cfg.transport == TransportKind.UnityRelay)
                return new UMK_UnityRelayTransport(); // Relay can work without Mirror (NGO/UTP path)
            onError?.Invoke("Mirror not present. Running in offline mode for non-Relay transports.");
            return new UMK_OfflineTransport();
#endif
        }
    }
}