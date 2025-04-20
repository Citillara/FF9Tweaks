using FF9;
using FF9Tweaks;
using FF9Tweaks.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FF9Tweaks.Patches
{
    [HarmonyPatch(typeof(ff9play), "FF9Play_SetParty")]
    class ff9play_FF9Play_SetParty_Patch
    {
        public static bool CanRun = true;

        static void Postfix(ref int party_id, ref int slot_id)
        {
            if (!CanRun)
                return;
            if (Main.Settings != null
                && Main.Settings.PartyEditorSettings != null
                && Main.Settings.PartyEditorSettings.Locked != null
                && Main.Settings.PartyEditorSettings.SelectedOption != null
                && Main.Settings.PartyEditorSettings.Locked[slot_id])
            {
                FF9Helpers.ForceAddCharacterToParty(Main.Settings.PartyEditorSettings.SelectedOption[slot_id], slot_id);
            }
        }
    }



    [HarmonyPatch(typeof(EventEngine), "partyadd")]
    class EventEngine_partyadd_Patch
    {
        public static bool CanRun = true;

        static void Postfix(ref long a)
        {
            if (Main.Settings != null
                && Main.Settings.PartyEditorSettings != null
                && Main.Settings.PartyEditorSettings.Locked != null
                && Main.Settings.PartyEditorSettings.SelectedOption != null)
            {
                int i = 0;
                foreach (bool locked in Main.Settings.PartyEditorSettings.Locked)
                {
                    if (locked)
                    {
                        ff9play_FF9Play_SetParty_Patch.CanRun = false;
                        FF9Helpers.ForceAddCharacterToParty(Main.Settings.PartyEditorSettings.SelectedOption[i], i);
                        ff9play_FF9Play_SetParty_Patch.CanRun = true;
                    }
                    i++;
                }
            }
        }
    }
}