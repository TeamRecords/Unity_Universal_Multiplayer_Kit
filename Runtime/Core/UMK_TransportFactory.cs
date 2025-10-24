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
                case TransportKind.UnityRelay:
#if HAS_UTP && HAS_UGS_RELAY
                    return new UMK_UnityRelayTransport();
#else
                    onError?.Invoke("Unity Relay selected, but required packages are missing. Install com.unity.transport and com.unity.services.relay.");
                    return new UMK_OfflineTransport();
#endif
                default: return new UMK_MirrorAutoTransport();
            }
#else
            if (cfg.transport == TransportKind.UnityRelay)
            {
#if HAS_UTP && HAS_UGS_RELAY
                return new UMK_UnityRelayTransport();
#else
                onError?.Invoke("Unity Relay selected, but required packages are missing.");
                return new UMK_OfflineTransport();
#endif
            }
            onError?.Invoke("Mirror not present. Running in offline mode for non-Relay transports.");
            return new UMK_OfflineTransport();
#endif
        }
    }
}