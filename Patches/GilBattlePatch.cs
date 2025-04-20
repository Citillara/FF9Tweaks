using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FF9Tweaks.Patches
{
    [HarmonyPatch(typeof(btl_para), "SetDamage", MethodType.Normal)]
    internal static class GilBattlePatch
    {
        static int hpHolder = 0;

        static void Prefix(ref BTL_DATA btl, ref int damage, ref byte dmg_mot)
        {
            POINTS cur = btl.cur;
            hpHolder = cur.hp;
            bool isPlayer = btl.bi.player == 0;
            
        }


        static void Postfix(ref BTL_DATA btl, ref int damage, ref byte dmg_mot)
        {
            POINTS cur = btl.cur;
            bool isPlayer = btl.bi.player == 0;

            // Ignore damage made by player
            if (isPlayer)
                return;

            if (Main.Settings.EnableGilLoss)
            {
                if (hpHolder > btl.cur.hp)
                {
                    uint initgil = FF9StateSystem.Common.FF9.party.gil;
                    FF9StateSystem.Common.FF9.party.gil -= damage > 0 ? (uint)damage : 0;
                    if (FF9StateSystem.Common.FF9.party.gil < 0)
                    {
                        FF9StateSystem.Common.FF9.party.gil = 0;
                    }
                }
            }
        }
    }
}
