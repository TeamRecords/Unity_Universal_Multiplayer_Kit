using UnityEngine;
using UMK.Core.Environment;

/// <summary>
/// Demonstrates how to control the DayNightCycle and WeatherSystem at runtime.
/// Attach this script to an empty GameObject and assign the DayNightCycle and WeatherSystem components via inspector.
/// </summary>
public class EnvironmentExample : MonoBehaviour
{
    public DayNightCycle dayNight;
    public WeatherSystem weather;

    void Update()
    {
        if (weather != null)
        {
            // Press 1–5 to switch weather states
            if (Input.GetKeyDown(KeyCode.Alpha1)) weather.SetWeather(WeatherSystem.WeatherType.Clear);
            if (Input.GetKeyDown(KeyCode.Alpha2)) weather.SetWeather(WeatherSystem.WeatherType.Cloudy);
            if (Input.GetKeyDown(KeyCode.Alpha3)) weather.SetWeather(WeatherSystem.WeatherType.Rain);
            if (Input.GetKeyDown(KeyCode.Alpha4)) weather.SetWeather(WeatherSystem.WeatherType.Fog);
            if (Input.GetKeyDown(KeyCode.Alpha5)) weather.SetWeather(WeatherSystem.WeatherType.Storm);
        }
        if (dayNight != null)
        {
            // Press -/+ to adjust time of day
            if (Input.GetKey(KeyCode.Minus))
            {
                float t = dayNight.GetNormalizedTime();
                dayNight.SetNormalizedTime(t - Time.deltaTime / dayNight.dayLengthInSeconds);
            }
            if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus))
            {
                float t = dayNight.GetNormalizedTime();
                dayNight.SetNormalizedTime(t + Time.deltaTime / dayNight.dayLengthInSeconds);
            }
        }
    }
}