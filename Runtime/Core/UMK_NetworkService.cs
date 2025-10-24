using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_NetworkService : MonoBehaviour
    {
        public static UMK_NetworkService Instance { get; private set; }

        [SerializeField] UMK_NetworkConfig config;
        ITransport _transport;
        IAntiCheatProvider _ac;
        IDiagnosticsProvider _diag;

        public ITransport Transport => _transport;
        public IAntiCheatProvider AntiCheat => _ac;
        public IDiagnosticsProvider Diagnostics => _diag;
        public UMK_NetworkConfig Config => config;

        /// <summary>
        /// Indicates whether this instance is running in server mode or in offline (no network) mode.
        /// When Mirror is present this checks the NetworkServer.active flag. Without Mirror it always
        /// returns true so that server‑only logic executes in local/offline runs. Use this property
        /// from non-networked scripts (e.g. examples) to gate actions that should only occur on the
        /// host or in offline scenarios.
        /// </summary>
#if HAS_MIRROR
        public bool IsServerOrOffline => Mirror.NetworkServer.active;
#else
        public bool IsServerOrOffline => true;
#endif

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!config) config = ScriptableObject.CreateInstance<UMK_NetworkConfig>();

            var overlay = new GameObject("UMK_Diagnostics", typeof(UMK.Runtime.UMK_DiagnosticsOverlay));
            overlay.transform.SetParent(transform, false);
            _diag = overlay.GetComponent<UMK.Runtime.UMK_DiagnosticsOverlay>();
            _diag.SetOverlayVisible(config.showDiagnosticsOverlay);

            _transport = UMK_TransportFactory.Create(config, Error);
            _transport.Initialize(gameObject, config, Error);
            _diag.SetTransport(_transport);

            _ac = UMK_AntiCheatFactory.Create(config, Error);
            if (_ac != null && _ac.Available)
            {
                _ac.Initialize(gameObject, config, Error);
                _ac.Enable();
                _ac.OnViolation += (msg,conf)=> {
                    if (conf > 0) Debug.LogWarning($"[UMK][AC] {msg} ({conf}%)");
                    else Debug.Log($"[UMK][AC] {msg}");
                };
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(config.toggleOverlayKey))
                _diag.Toggle();
        }

        public void StartHost(){ _transport.StartHost(); }
        public void StartClient(string addrOrCode){ _transport.StartClient(addrOrCode); }
        public void StartServer(){ _transport.StartServer(); }

        void Error(string message){ Debug.LogError($"[UMK] {message}"); }
    }
}