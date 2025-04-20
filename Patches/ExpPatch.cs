using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace FF9Tweaks.Patches
{
    [HarmonyPatch(typeof(FF9.btl_sys), "SetBonus", MethodType.Normal)]
    internal static class ExpPatch
    {
        static void Prefix(ref ENEMY_TYPE et)
        {
            if (Main.Settings.EnableNoXP)
                et.bonus.exp = 0;
        }
    }
}
