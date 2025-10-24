using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_DefaultValidationAC : IAntiCheatProvider
    {
        public string Name => "Default Validation";
        public bool Available => true;
        public event Action<string,int> OnViolation;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            var _ = OnViolation; // avoid CS0067 without spamming logs
        }
        public void Enable(){}
        public void Disable(){}
        public void Report(string message, int confidence=80) => OnViolation?.Invoke(message, confidence);
    }
}