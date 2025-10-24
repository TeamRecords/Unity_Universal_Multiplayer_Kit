using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Environment
{
    /// <summary>
    /// Simple day/night cycle that replicates the time of day across the network when Mirror is present.
    /// Without Mirror, the cycle runs locally. The cycle can drive a directional light to simulate the sun.
    /// </summary>
    public class DayNightCycle : NetBehaviourBase
    {
        [Header("Cycle Settings")]
        [Tooltip("Length of a full day in seconds.")]
        public float dayLengthInSeconds = 600f;

        [Tooltip("Directional light representing the sun.")]
        public Light sunLight;

        [Tooltip("Sun intensity at noon.")]
        public float maxIntensity = 1f;

        [Tooltip("Sun intensity at midnight.")]
        public float minIntensity = 0.1f;

        // Normalized time [0,1) where 0 is midnight and 0.5 is noon
        #if HAS_MIRROR
        [SyncVar]
        #endif
        private float normalizedTime;

        private void Start()
        {
            // Initialize time so that the host starts at 6am (0.25)
            if (IsServerOrOffline())
            {
                normalizedTime = 0.25f;
            }
        }

        private void Update()
        {
            if (IsServerOrOffline())
            {
                // Advance time based on day length
                normalizedTime += Time.deltaTime / dayLengthInSeconds;
                normalizedTime %= 1f;
            }

            // Update sun light if assigned
            if (sunLight)
            {
                UpdateSunLight();
            }
        }

        private void UpdateSunLight()
        {
            // Sun rotates 360 degrees over a full day
            float rotation = normalizedTime * 360f;
            sunLight.transform.localRotation = Quaternion.Euler(new Vector3(rotation - 90f, 170f, 0f));

            // Adjust intensity based on time
            // 0 at midnight, 1 at noon; use cosine for smooth transition
            float t = Mathf.Cos(normalizedTime * Mathf.PI * 2f) * -0.5f + 0.5f;
            sunLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

        /// <summary>
        /// Gets the current normalized time of day (0 to 1).
        /// </summary>
        public float GetNormalizedTime() => normalizedTime;

        /// <summary>
        /// Sets the normalized time on the server. Clients will be updated via SyncVar.
        /// </summary>
        public void SetNormalizedTime(float value)
        {
            if (IsServerOrOffline())
            {
                normalizedTime = Mathf.Repeat(value, 1f);
            }
        }
    }
}