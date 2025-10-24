using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
#if ENABLE_INPUT_SYSTEM
            // When using the new Input System, UnityEngine.Input.GetKeyDown is invalid. Use the InputSystem API instead.
            if (Keyboard.current != null && config != null)
            {
                // Try to parse the KeyCode name into an InputSystem Key enum; fallback to F9 if parsing fails.
                var keyName = config.toggleOverlayKey.ToString();
                if (System.Enum.TryParse<UnityEngine.InputSystem.Key>(keyName, out var sysKey))
                {
                    if (Keyboard.current[sysKey].wasPressedThisFrame)
                        _diag.Toggle();
                }
                else
                {
                    if (Keyboard.current[UnityEngine.InputSystem.Key.F9].wasPressedThisFrame)
                        _diag.Toggle();
                }
            }
#else
            // Legacy Input Manager: safe to call Input.GetKeyDown
            if (Input.GetKeyDown(config.toggleOverlayKey))
                _diag.Toggle();
#endif
        }

        public void StartHost(){ _transport.StartHost(); }
        public void StartClient(string addrOrCode){ _transport.StartClient(addrOrCode); }
        public void StartServer(){ _transport.StartServer(); }

        void Error(string message)
        {
            // Downgrade the "Mirror not present" message to an info log instead of an error.
            if (!string.IsNullOrEmpty(message) && message.Contains("Mirror not present"))
            {
                Debug.Log($"[UMK] {message}");
            }
            else
            {
                Debug.LogError($"[UMK] {message}");
            }
        }
    }
}