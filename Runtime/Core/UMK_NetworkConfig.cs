using UnityEngine;

namespace UMK.Core
{
    public enum TransportKind { Auto, Telepathy, Kcp, SteamSDR, UnityRelay }
    public enum AntiCheatKind { None, DefaultValidation, UnityGameShield, Guard, VAC, EAC, BattleEye }

    [CreateAssetMenu(menuName = "UMK/Network Config", fileName = "UMK_NetworkConfig")]
    public class UMK_NetworkConfig : ScriptableObject
    {
        public TransportKind transport = TransportKind.Auto;
        public AntiCheatKind antiCheat = AntiCheatKind.DefaultValidation;
        public bool showDiagnosticsOverlay = true;
        public KeyCode toggleOverlayKey = KeyCode.F9;

        [Header("Steam (SDR)")]
        public bool createSteamTransportIfMissing = true;

        [Header("Unity Relay")]
        public string joinCode = "";
        public int maxRelayConnections = 8;
    }
}