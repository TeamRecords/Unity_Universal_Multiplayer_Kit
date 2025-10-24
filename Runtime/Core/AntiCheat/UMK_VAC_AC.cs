using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_VAC_AC : IAntiCheatProvider
    {
        public string Name => "VAC";
        public bool Available => Type.GetType("Steamworks.SteamClient, Facepunch.Steamworks") != null || Type.GetType("Steamworks.SteamAPI, Steamworks.NET") != null;
        public event Action<string,int> OnViolation;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            if (!Available) onError?.Invoke("Steamworks not detected (VAC requires Steam).");
        }
        public void Enable(){}
        public void Disable(){}
    }
}