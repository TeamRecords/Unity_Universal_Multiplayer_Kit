using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Environment
{
    /// <summary>
    /// Simple weather system that replicates the current weather type across the network when Mirror is present.
    /// Weather types can be extended by users. Without Mirror, the system runs locally.
    /// </summary>
    public class WeatherSystem : NetBehaviourBase
    {
        public enum WeatherType
        {
            Clear,
            Cloudy,
            Rain,
            Fog,
            Storm
        }

        [Header("Weather Settings")]
        [Tooltip("Current weather type.")]
        #if HAS_MIRROR
        [SyncVar(hook = nameof(OnWeatherChanged))]
        #endif
        public WeatherType currentWeather = WeatherType.Clear;

        [Tooltip("Particle system for rain.")]
        public ParticleSystem rainParticle;

        [Tooltip("Particle system for fog.")]
        public ParticleSystem fogParticle;

        [Tooltip("Audio source for storm sound.")]
        public AudioSource stormAudio;

        private void Start()
        {
            // Initialize weather on server/offline
            if (IsServerOrOffline())
            {
                ApplyWeatherEffects(currentWeather);
            }
        }

        /// <summary>
        /// Changes the weather type. Only permitted on server/offline.
        /// </summary>
        public void SetWeather(WeatherType newWeather)
        {
            if (IsServerOrOffline())
            {
                currentWeather = newWeather;
                // OnWeatherChanged will be invoked via SyncVar hook on all clients including host
                OnWeatherChanged(currentWeather, currentWeather);
            }
        }

        // Mirror SyncVar hook signature (oldValue unused)
        private void OnWeatherChanged(WeatherType oldWeather, WeatherType newWeather)
        {
            ApplyWeatherEffects(newWeather);
        }

        private void ApplyWeatherEffects(WeatherType weather)
        {
            // Enable/disable particle systems and audio based on weather
            if (rainParticle)
            {
                var emission = rainParticle.emission;
                emission.enabled = (weather == WeatherType.Rain || weather == WeatherType.Storm);
            }
            if (fogParticle)
            {
                var emission = fogParticle.emission;
                emission.enabled = (weather == WeatherType.Fog);
            }
            if (stormAudio)
            {
                if (weather == WeatherType.Storm)
                {
                    stormAudio.loop = true;
                    if (!stormAudio.isPlaying) stormAudio.Play();
                }
                else
                {
                    if (stormAudio.isPlaying) stormAudio.Stop();
                }
            }
        }
    }
}