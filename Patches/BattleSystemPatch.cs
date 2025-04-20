using Assets.Scripts.Common;
using FF9;
using FF9Tweaks.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyJson;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace FF9Tweaks.Patches
{
    static class BattleSystemDataHolder
    {
        public static bool NewBattleFlag = false;
        public static int CurrentStatsPenalty = 0;
    }

    [HarmonyPatch(typeof(battle), "InitBattle")]
    static class BattleSystemInitPatch
    {
        static void Prefix()
        {

            BattleSystemDataHolder.NewBattleFlag = true;

            if (Main.Settings.StatsPenaltySettings.EnableStatPenalty)
            {
                BattleSystemDataHolder.CurrentStatsPenalty = 0;

                if (Main.Settings.StatsPenaltySettings.EnableRawStatsPenalty)
                    BattleSystemDataHolder.CurrentStatsPenalty += Main.Settings.StatsPenaltySettings.RawStatsPenalty;

                if (Main.Settings.StatsPenaltySettings.EnableStatPenaltyBaseOnFile)
                {
                    if (!string.IsNullOrEmpty(Main.Settings.StatsPenaltySettings.StatPenaltyBaseFilePath))
                    {
                        if (File.Exists(Main.Settings.StatsPenaltySettings.StatPenaltyBaseFilePath))
                        {
                            if (Main.Settings.StatsPenaltySettings.StatPenaltyBaseFileType == Tools.StatsPenaltySettings.FileType.INI)
                            {
                                Tools.IniFile file = new Tools.IniFile(Main.Settings.StatsPenaltySettings.StatPenaltyBaseFilePath);
                                string s = file.Read("count", "viewer").Replace("\"", "");
                                int i = 0;
                                if (int.TryParse(s, out i))
                                {
                                    BattleSystemDataHolder.CurrentStatsPenalty += i;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(battle), "BattleMainLoop")]
    static class BattleSystemPatch
    {
        static DateTime last = DateTime.Now;

        static void Prefix(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (BattleSystemDataHolder.NewBattleFlag)
                {
                    BattleSystemDataHolder.NewBattleFlag = false;
                    DoDebugLogOnce(ref sys, ref btlsys, now);
                    DoSoloModeOnce(ref sys, ref btlsys, now);
                    DoStatsPenaltyOnce(ref sys, ref btlsys, now);
                    DoHPLossOnce(ref sys, ref btlsys, now);
                }

                DoHPLossLoop(ref sys, ref btlsys, now);
            }
            catch (Exception e)
            {
                Main.Logger.LogException(e);
                Main.Logger.Error(e.ToString());
            }
        }

        static void DoDebugLogOnce(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            try
            {
                if (!Main.Settings.EnableBattleLog)
                {
                    return;
                }

                /*if (!Directory.Exists("c:\a"))
                    Directory.CreateDirectory("c:\a");*/

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("---START BATTLE DATA---");
                stringBuilder.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                stringBuilder.AppendLine(ToJson(btlsys));

                stringBuilder.AppendLine("---Entities list---");
                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                {
                    if (next.bi.player == 0) // Skip if not a player
                    {
                        stringBuilder.Append("MONSTER".ToString().PadRight(10));
                    }
                    else
                    {
                        PlayerType playerType = FF9Helpers.GetRealPlayerType(FF9StateSystem.Common.FF9.player[next.bi.slot_no]);
                        stringBuilder.Append(playerType.ToString().PadRight(10));
                    }
                    stringBuilder.Append(DumpData(next));
                    stringBuilder.Append("bi : " + ToJson(next.stat) + ",");
                    stringBuilder.Append("stat : " + ToJson(next.elem) + ",");
                    stringBuilder.Append("elem : " + ToJson(next.cur) + ",");
                    stringBuilder.Append("cur : " + ToJson(next.max) + ",");
                    stringBuilder.Append("max : " + ToJson(next.weapon) + ",");
                    stringBuilder.Append("weapon : " + ToJson(next.defence) + ",");
                    stringBuilder.Append("defence : " + ToJson(next.def_attr) + ",");

                    stringBuilder.AppendLine("---");
                }
                stringBuilder.AppendLine("---END BATTLE DATA---");

                //File.AppendAllText(filePath, stringBuilder.ToString());

                Main.Logger.Log(stringBuilder.ToString());
            }
            catch (Exception e)
            {
                Main.Logger.LogException(e);
                Main.Logger.Error(e.ToString());
            }
        }

        static string DumpData(BTL_DATA data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"btl_id: {data.btl_id}, ");
            stringBuilder.Append($"trance: {data.trance}, ");
            stringBuilder.Append($"p_up_attr: {data.p_up_attr}, ");
            stringBuilder.Append($"level: {data.level}, ");
            stringBuilder.Append($"escape_key: {data.escape_key}, ");
            stringBuilder.Append($"tar_bone: {data.tar_bone}, ");
            stringBuilder.Append($"die_seq: {data.die_seq}, ");
            stringBuilder.Append($"fig_info: {data.fig_info}, ");
            stringBuilder.Append($"fig: {data.fig}, ");
            stringBuilder.Append($"m_fig: {data.m_fig}, ");
            stringBuilder.Append($"dms_geo_id: {data.dms_geo_id}, ");
            stringBuilder.Append($"mesh_current: {data.mesh_current}, ");
            stringBuilder.Append($"mesh_banish: {data.mesh_banish}, ");
            stringBuilder.Append($"tar_mode: {data.tar_mode}, ");
            stringBuilder.Append($"sel_mode: {data.sel_mode}, ");
            stringBuilder.Append($"shadow_x: {data.shadow_x}, ");
            stringBuilder.Append($"shadow_z: {data.shadow_z}, ");
            stringBuilder.Append($"fig_regene_hp: {data.fig_regene_hp}, ");
            stringBuilder.Append($"fig_poison_hp: {data.fig_poison_hp}, ");
            stringBuilder.Append($"fig_poison_mp: {data.fig_poison_mp}, ");
            stringBuilder.Append($"fig_stat_info: {data.fig_stat_info}, ");
            stringBuilder.Append($"sel_menu: {data.sel_menu}, ");
            stringBuilder.Append($"typeNo: {data.typeNo}, ");
            stringBuilder.Append($"idleAnimationName: {data.idleAnimationName}, ");
            stringBuilder.Append($"flags: {data.flags}, ");
            stringBuilder.Append($"meshflags: {data.meshflags}, ");
            stringBuilder.Append($"weaponFlags: {data.weaponFlags}, ");
            stringBuilder.Append($"weaponMeshFlags: {data.weaponMeshFlags}, ");
            stringBuilder.Append($"meshCount: {data.meshCount}, ");
            stringBuilder.Append($"weaponMeshCount: {data.weaponMeshCount}, ");
            stringBuilder.Append($"attachOffset: {data.attachOffset}, ");
            stringBuilder.Append($"weaponIsRendering: {data.weaponIsRendering}, ");
            stringBuilder.Append($"battleModelIsRendering: {data.battleModelIsRendering}, ");
            stringBuilder.Append($"currentAnimationName: {data.currentAnimationName}, ");
            stringBuilder.Append($"height: {data.height}, ");
            stringBuilder.Append($"radius: {data.radius}, ");
            stringBuilder.Append($"frameCount: {data.frameCount}, ");
            stringBuilder.Append($"targetFrame: {data.targetFrame}, ");
            return stringBuilder.ToString();
        }

        static string ToJson(object obj)
        {
            if (obj == null)
                return "NULL";
            return obj.CToJson();
        }

        static void DoSoloModeOnce(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            if (!Main.Settings.SoloModeSettings.Enabled)
            {
                return;
            }

            bool Zidane = Main.Settings.SoloModeSettings.Zidane;
            bool Vivi = Main.Settings.SoloModeSettings.Vivi;
            bool Dagger = Main.Settings.SoloModeSettings.Dagger;
            bool Steiner = Main.Settings.SoloModeSettings.Steiner;
            bool Freya = Main.Settings.SoloModeSettings.Freya;
            bool Quina = Main.Settings.SoloModeSettings.Quina;
            bool Eiko = Main.Settings.SoloModeSettings.Eiko;
            bool Amarant = Main.Settings.SoloModeSettings.Amarant;
            bool Beatrix = Main.Settings.SoloModeSettings.Beatrix;

            // First, check if at least one character is set to keep alive
            bool shouldAdjustHp = Zidane || Vivi || Dagger || Steiner || Freya || Quina || Eiko || Amarant || Beatrix;

            // If at least one character is set to true, then proceed
            if (shouldAdjustHp)
            {
                bool keepAliveCharacterPresent = false;

                // Check if any of the characters set to keep alive is present on the battlefield
                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                {
                    if (next.bi.player == 0) // Skip if not a player
                        continue;

                    PlayerType playerType = FF9Helpers.GetPlayerType(FF9StateSystem.Common.FF9.player[next.bi.slot_no]);

                    // Check if this character is set to keep alive
                    if ((playerType == PlayerType.Zidane && Zidane) ||
                        (playerType == PlayerType.Vivi && Vivi) ||
                        (playerType == PlayerType.Dagger && Dagger) ||
                        (playerType == PlayerType.Steiner && Steiner) ||
                        (playerType == PlayerType.Freya && Freya) ||
                        (playerType == PlayerType.Quina && Quina) ||
                        (playerType == PlayerType.Eiko && Eiko) ||
                        (playerType == PlayerType.Amarant && Amarant) ||
                        (playerType == PlayerType.Beatrix && Beatrix))
                    {
                        keepAliveCharacterPresent = true;
                        break; // Found at least one, no need to continue checking
                    }
                }

                // If a character set to keep alive is present, adjust HP for others
                if (keepAliveCharacterPresent)
                {
                    for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                    {
                        if (next.bi.player == 0) // Skip if not a player
                            continue;

                        PlayerType playerType = FF9Helpers.GetPlayerType(FF9StateSystem.Common.FF9.player[next.bi.slot_no]);

                        // Check if this character should not be kept alive and adjust HP accordingly
                        bool isCharacterToKill = !((playerType == PlayerType.Zidane && Zidane) ||
                                                   (playerType == PlayerType.Vivi && Vivi) ||
                                                   (playerType == PlayerType.Dagger && Dagger) ||
                                                   (playerType == PlayerType.Steiner && Steiner) ||
                                                   (playerType == PlayerType.Freya && Freya) ||
                                                   (playerType == PlayerType.Quina && Quina) ||
                                                   (playerType == PlayerType.Eiko && Eiko) ||
                                                   (playerType == PlayerType.Amarant && Amarant) ||
                                                   (playerType == PlayerType.Beatrix && Beatrix));

                        if (isCharacterToKill)
                        {
                            next.cur.hp = 0;
                        }
                    }
                }
            }

        }

        // Raw stats penalty system. No much used
        static void DoStatsPenaltyOnce(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            if (!Main.Settings.StatsPenaltySettings.EnableStatPenalty)
            {
                return;
            }
            if (BattleSystemDataHolder.CurrentStatsPenalty > 0)
            {
                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                {
                    // If not a player, ignore. Yes here it's non-zero for a player, don't ask ...
                    if (next.bi.player == 0)
                        continue;

                    next.elem.wpr = ComputeStatsPenalty(next.elem.wpr);
                    next.elem.str = ComputeStatsPenalty(next.elem.str);
                    next.elem.dex = ComputeStatsPenalty(next.elem.dex);
                    next.elem.mgc = ComputeStatsPenalty(next.elem.mgc);
                }
            }

        }

        static void DoHPLossOnce(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            if (!Main.Settings.HPLossSettings.Enabled)
            {
                return;
            }
            for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
            {
                // If not a player, ignore. Yes here it's non-zero for a player, don't ask ...
                if (next.bi.player == 0)
                    continue;

                // Inverted logic here
                // Not using abilities is basically having maximum capa points, i.e max.capa == current.capa as activating an
                // ability "drains" the capa pool
                bool usedcapa = next.max.capa != next.cur.capa;
                if (usedcapa)
                {
                    // Has used capa -> don't half stats
                    continue;
                }

                // Character didn't activated any abilities/capa -> No HP/MP loss over time but halfing the stats
                next.elem.wpr = (byte)(next.elem.wpr / 2);
                next.elem.str = (byte)(next.elem.str / 2);
                next.elem.dex = (byte)(next.elem.dex / 2);
                next.elem.mgc = (byte)(next.elem.mgc / 2);
            }

            DoHPLossOnceFileOutput(ref sys, ref btlsys, now);
        }

        static void DoHPLossOnceFileOutput(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            if (Main.Settings.HPLossSettings.OutputToFile)
            {
                return;
            }
            if (!string.IsNullOrEmpty(Main.Settings.HPLossSettings.FilePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[character]");

                for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                {
                    // If not a player, ignore. Yes here it's non-zero for a player, don't ask ...
                    if (next.bi.player == 0)
                        continue;


                    string charname = FF9StateSystem.Common.FF9.player[next.bi.slot_no].name;
                    bool usedcapa = next.max.capa != next.cur.capa;
                    if (!usedcapa)
                    {
                        // Has not used capa -> do nothing

                        sb.AppendLine($"{charname}=0");
                        continue;
                    }
                    int hploss = 0;


                    switch (Main.Settings.HPLossSettings.LossFormula)
                    {
                        case Tools.HPLossSettings.HPLossFormula.APDividedBy10:
                            hploss = (next.max.capa - next.cur.capa) / 10;
                            break;
                        case Tools.HPLossSettings.HPLossFormula.APDividedBy10WithLevel:
                            hploss = ((next.max.capa - next.cur.capa) / 10);
                            hploss += FF9StateSystem.Common.FF9.player[next.bi.slot_no].level;
                            break;
                        case Tools.HPLossSettings.HPLossFormula.APPercentage:
                            int capaPointsUsed = next.max.capa - next.cur.capa;
                            double hpLossPercentPerCapaPoint = Main.Settings.HPLossSettings.HPAPLossPercent / 1000.0;
                            hploss = (int)(capaPointsUsed * hpLossPercentPerCapaPoint * next.max.hp);
                            break;
                        default: break;

                    }

                    sb.AppendLine($"{charname}={hploss}");
                }
                try
                {
                    File.WriteAllText(Main.Settings.HPLossSettings.FilePath, sb.ToString());
                }
                catch (Exception e)
                {
                    Main.mod.Logger.Log("Exception writing HPLossFile: " + e.ToString());
                }
            }
        }

        static void DoHPLossLoop(ref FF9StateGlobal sys, ref FF9StateBattleSystem btlsys, DateTime now)
        {
            if (Main.Settings.HPLossSettings.Enabled)
            {
                if (last.Ticks + TimeSpan.TicksPerSecond < now.Ticks)
                {
                    last = now;
                    for (BTL_DATA next = btlsys.btl_list.next; next != null; next = next.next)
                    {
                        // If not a player, ignore. Yes here it's non-zero for a player, don't ask ...
                        if (next.bi.player == 0)
                            continue;

                        if (next.cur.hp <= 0)
                            continue;

                        bool usedcapa = next.max.capa != next.cur.capa;
                        if (!usedcapa)
                        {
                            // Has not used capa -> do nothing
                            continue;
                        }
                        int hploss = 0;


                        switch (Main.Settings.HPLossSettings.LossFormula)
                        {
                            case Tools.HPLossSettings.HPLossFormula.APDividedBy10:
                                hploss = (next.max.capa - next.cur.capa) / 10;
                                break;
                            case Tools.HPLossSettings.HPLossFormula.APDividedBy10WithLevel:
                                hploss = ((next.max.capa - next.cur.capa) / 10);
                                hploss += FF9StateSystem.Common.FF9.player[next.bi.slot_no].level;
                                break;
                            case Tools.HPLossSettings.HPLossFormula.APPercentage:
                                int capaPointsUsed = next.max.capa - next.cur.capa;
                                double hpLossPercentPerCapaPoint = Main.Settings.HPLossSettings.HPAPLossPercent / 1000.0;
                                hploss = (int)(capaPointsUsed * hpLossPercentPerCapaPoint * next.max.hp);
                                break;
                            default: break;

                        }
                        if (hploss <= 0)
                        {
                            hploss = 1;
                        }

                        int mploss = hploss / 10;
                        if (mploss <= 0)
                        {
                            mploss = 1;
                        }


                        if (next.cur.mp > 0)
                        {
                            if (next.cur.mp > mploss)
                            {
                                next.cur.mp -= (short)mploss;
                            }
                            else
                            {
                                next.cur.mp = 0;
                            }
                        }
                        else
                        {
                            hploss = hploss * 2;
                        }

                        if (next.cur.hp > 0)
                        {
                            if (next.cur.hp > hploss)
                            {
                                next.cur.hp -= (ushort)hploss;
                            }
                            else
                            {
                                next.cur.hp = 0;
                            }
                        }

                    }

                }

            }
        }

        static byte ComputeStatsPenalty(byte originalValue)
        {
            if (BattleSystemDataHolder.CurrentStatsPenalty >= originalValue)
            {
                return 1;
            }
            else
            {
                return (byte)(originalValue - (byte)BattleSystemDataHolder.CurrentStatsPenalty);
            }
        }
    }

    //[HarmonyPatch(typeof(SceneDirector), "FF9Wipe_FadeOutEx")]
    //static class Debug1
    //{
    //    static DateTime last = DateTime.Now;
    //
    //    static void Prefix(ref int frame)
    //    {
    //        //File.AppendAllText(@"c:\a\ff9.log", $"{DateTime.Now} - FF9Wipe_FadeOutEx" + Environment.NewLine);
    //    }
    //}
    //[HarmonyPatch(typeof(btl_sys), "ManageBattleEnd")]
    //static class Debug2
    //{
    //    static DateTime last = DateTime.Now;
    //
    //    static void Prefix(ref FF9StateBattleSystem btlsys)
    //    {
    //        //File.AppendAllText(@"c:\a\ff9.log", $"{DateTime.Now} - ManageBattleEnd" + Environment.NewLine);
    //    }
    //}



    //[HarmonyPatch(typeof(btl_cmd), "CheckCommandLoop")]
    //static class DebugCheckCommandLoop
    //{
    //    static DateTime last = DateTime.Now;
    //
    //    static void Prefix(ref CMD_DATA cmd)
    //    {
    //        try
    //        {
    //            if (!Main.Settings.EnableBattleLog)
    //            {
    //                return;
    //            }
    //
    //            if (cmd == null)
    //                return;
    //
    //            File.AppendAllText(@"c:\a\battlelog.txt", $"{DateTime.Now} - CheckCommandLoop  - NEW COMMAND - " + cmd.ToJson() + Environment.NewLine);
    //        }
    //        catch (Exception e)
    //        {
    //            Main.Logger.LogException(e);
    //            Main.Logger.Error(e.ToString());
    //        }
    //    }
    //}
}