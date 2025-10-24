using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_EAC_AC : IAntiCheatProvider
    {
        public string Name => "EAC";
        public bool Available => Type.GetType("EasyAntiCheat.Client.Hydra, EasyAntiCheat.Client") != null;
        public event Action<string,int> OnViolation;

        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            // Touch event to avoid CS0067 'never used' warning
            var _ = OnViolation;
            if (!Available) onError?.Invoke("EAC not detected in project.");
        }

        public void Enable(){ /* hook provider here if present */ }
        public void Disable(){ }
    }
}