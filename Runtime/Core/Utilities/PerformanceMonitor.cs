using UnityEngine;

namespace UMK.Core.Utilities
{
    /// <summary>
    /// Displays performance statistics like FPS and approximate CPU usage. Can be toggled on/off at runtime.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Tooltip("Key used to toggle the monitor on/off.")]
        public KeyCode toggleKey = KeyCode.F10;
        public bool show;

        private float deltaTime;

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                show = !show;
            }
            if (!show) return;
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            if (!show) return;
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(10, 10, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 14;
            style.normal.textColor = Color.white;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}