using System;
using UnityEngine;

namespace UMK.Core
{
    public class UMK_BattleEye_AC : IAntiCheatProvider
    {
        public string Name => "BattleEye";
        public bool Available => Type.GetType("BattlEye.BEClient, BattlEye") != null;
        public event Action<string,int> OnViolation;
        public void Initialize(GameObject context, UMK_NetworkConfig config, Action<string> onError)
        {
            if (!Available) onError?.Invoke("BattlEye not detected in project.");
        }
        public void Enable(){}
        public void Disable(){}
    }
}