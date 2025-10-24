#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UMK.Core;

public class UMK_SetupWizard : EditorWindow
{
    UMK_NetworkConfig config;
    [MenuItem("Tools/UMK/Setup Wizard")]
    public static void Open(){ GetWindow<UMK_SetupWizard>("UMK Setup"); }

    void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("t:UMK_NetworkConfig");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            config = AssetDatabase.LoadAssetAtPath<UMK_NetworkConfig>(path);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Universal Multiplayer Kit", EditorStyles.boldLabel);
        config = (UMK_NetworkConfig)EditorGUILayout.ObjectField("Config Asset", config, typeof(UMK_NetworkConfig), false);
        if (!config)
        {
            if (GUILayout.Button("Create Config Asset"))
            {
                var inst = ScriptableObject.CreateInstance<UMK_NetworkConfig>();
                string path = "Assets/UMK_NetworkConfig.asset";
                AssetDatabase.CreateAsset(inst, path);
                AssetDatabase.SaveAssets();
                config = AssetDatabase.LoadAssetAtPath<UMK_NetworkConfig>(path);
            }
            return;
        }

        config.transport = (TransportKind)EditorGUILayout.EnumPopup("Transport", config.transport);
        if (config.transport == TransportKind.SteamSDR)
        {
            EditorGUILayout.HelpBox("Requires a Steam Sockets transport (SteamSocketsTransport/FizzySteamworks) on NetworkManager.", MessageType.Info);
            config.createSteamTransportIfMissing = EditorGUILayout.Toggle("Auto-attach Steam transport", config.createSteamTransportIfMissing);
        }
        if (config.transport == TransportKind.UnityRelay)
        {
            EditorGUILayout.HelpBox("Requires: Services Core + Relay + Unity Transport + NGO NetworkManager.", MessageType.Info);
            config.joinCode = EditorGUILayout.TextField("Join Code (client)", config.joinCode);
            config.maxRelayConnections = EditorGUILayout.IntField("Max Clients (host)", config.maxRelayConnections);
        }

        config.antiCheat = (AntiCheatKind)EditorGUILayout.EnumPopup("Anti-Cheat", config.antiCheat);
        config.showDiagnosticsOverlay = EditorGUILayout.Toggle("Show Diagnostics Overlay", config.showDiagnosticsOverlay);
        config.toggleOverlayKey = (KeyCode)EditorGUILayout.EnumPopup("Toggle Key", config.toggleOverlayKey);

        EditorGUILayout.Space();
        if (GUILayout.Button("Run Environment Checks")) RunChecks(config);
        if (GUI.changed) EditorUtility.SetDirty(config);
    }

    void RunChecks(UMK_NetworkConfig cfg)
    {
#if HAS_MIRROR
        Debug.Log("[UMK] Mirror detected.");
#else
        Debug.LogWarning("[UMK] Mirror NOT detected. Mirror transports will run in offline shim or be unavailable.");
#endif

        if (cfg.transport == TransportKind.SteamSDR)
        {
            bool found = FindType("SteamSocketsTransport") != null ||
                         FindType("FizzySteamworks") != null ||
                         FindType("FacepunchTransport") != null;
            if (!found) EditorUtility.DisplayDialog("UMK", "No Steam transport found. Add a Steam Sockets transport to NetworkManager.", "OK");
            else Debug.Log("[UMK] Steam transport type found.");
        }

        if (cfg.transport == TransportKind.UnityRelay)
        {
            bool services = FindType("Unity.Services.Core.UnityServices") != null;
            bool relay = FindType("Unity.Services.Relay.RelayService") != null;
            bool utp = FindType("Unity.Netcode.Transports.UTP.UnityTransport") != null || FindType("UnityTransport") != null;
            bool ngo = FindType("Unity.Netcode.NetworkManager") != null;
            if (!services || !relay || !utp || !ngo)
                EditorUtility.DisplayDialog("UMK", "Relay requirements missing (Services Core + Relay + UTP + NGO).", "OK");
            else Debug.Log("[UMK] Unity Relay prerequisites present.");
        }

        CheckAC(cfg);
    }

    void CheckAC(UMK_NetworkConfig cfg)
    {
        switch (cfg.antiCheat)
        {
            case AntiCheatKind.UnityGameShield: ReportPresence("UnityGameShield.EntryPoint, UnityGameShield", "Unity Game Shield"); break;
            case AntiCheatKind.Guard: ReportPresence("Guard.Core.Entry, Guard", "GUARD"); break;
            case AntiCheatKind.VAC:
                bool vac = FindType("Steamworks.SteamClient, Facepunch.Steamworks") != null ||
                           FindType("Steamworks.SteamAPI, Steamworks.NET") != null;
                if (!vac) EditorUtility.DisplayDialog("UMK", "Steamworks not found (VAC requires Steam).", "OK");
                else Debug.Log("[UMK] Steamworks present.");
                break;
            case AntiCheatKind.EAC: ReportPresence("EasyAntiCheat.Client.Hydra, EasyAntiCheat.Client", "EAC"); break;
            case AntiCheatKind.BattleEye: ReportPresence("BattlEye.BEClient, BattlEye", "BattlEye"); break;
            default: Debug.Log("[UMK] Default Validation selected."); break;
        }
    }

    static System.Type FindType(string fullName)
    {
        foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            System.Type t = null;
            try { t = asm.GetType(fullName, false); } catch {}
            if (t != null) return t;
        }
        return null;
    }

    static void ReportPresence(string typeName, string label)
    {
        if (FindType(typeName) == null)
            EditorUtility.DisplayDialog("UMK", label + " not found in project.", "OK");
        else
            Debug.Log("[UMK] " + label + " present.");
    }
}
#endif