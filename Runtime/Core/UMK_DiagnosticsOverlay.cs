using UnityEngine;
using UnityEngine.UI;
using UMK.Core;

namespace UMK.Runtime
{
    public class UMK_DiagnosticsOverlay : MonoBehaviour, IDiagnosticsProvider
    {
        [SerializeField] Text label;
        [SerializeField] Canvas canvas;
        ITransport _transport;
        float _fpsAccum;
        int _frames;
        float _timer;

        void Awake()
        {
            if (!canvas)
            {
                var go = new GameObject("UMK_OverlayCanvas");
                go.transform.SetParent(transform, false);
                canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var textGO = new GameObject("Label");
                textGO.transform.SetParent(go.transform, false);
                label = textGO.AddComponent<Text>();
                label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                label.fontSize = 14;
                label.alignment = TextAnchor.UpperLeft;
                var rt = label.rectTransform; rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1); rt.anchoredPosition = new Vector2(10,-10);
            }
        }

        void Update()
        {
            _fpsAccum += Time.unscaledDeltaTime;
            _frames++;
            _timer += Time.unscaledDeltaTime;
            if (_timer >= 0.5f)
            {
                float fps = _frames / _fpsAccum;
                int ping = _transport != null ? _transport.GetPingMs() : 0;
                label.text = $"UMK • FPS: {fps:0} • Ping: {ping} ms • Transport: {(_transport!=null?_transport.Name:"None")}";
                _timer = 0; _frames = 0; _fpsAccum = 0;
            }
        }

        public void SetOverlayVisible(bool visible){ if (canvas) canvas.enabled = visible; }
        public void Toggle(){ if (canvas) canvas.enabled = !canvas.enabled; }
        public void SetTransport(ITransport transport){ _transport = transport; }
    }
}