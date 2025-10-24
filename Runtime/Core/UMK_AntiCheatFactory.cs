using System;
using UnityEngine;

namespace UMK.Core
{
    public static class UMK_AntiCheatFactory
    {
        public static IAntiCheatProvider Create(UMK_NetworkConfig cfg, Action<string> onError)
        {
            switch (cfg.antiCheat)
            {
                case AntiCheatKind.DefaultValidation: return new UMK_DefaultValidationAC();
                case AntiCheatKind.UnityGameShield: return new UMK_UnityGameShieldAC();
                case AntiCheatKind.Guard: return new UMK_GuardAC();
                case AntiCheatKind.VAC: return new UMK_VAC_AC();
                case AntiCheatKind.EAC: return new UMK_EAC_AC();
                case AntiCheatKind.BattleEye: return new UMK_BattleEye_AC();
                default: return null;
            }
        }
    }
}