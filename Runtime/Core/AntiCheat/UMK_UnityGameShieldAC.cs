using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_UnityGameShieldAC : IAntiCheatProvider
    {
        public string Name => "UnityGameShieldAC";
        public bool Available => Type.GetType("UnityGameShield.EntryPoint, UnityGameShield") != null;
        public event Action<string,int> OnViolation;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            if (!Available) onError?.Invoke("Unity Game Shield not detected in project.");
        }
        public void Enable(){}
        public void Disable(){}
    }
}