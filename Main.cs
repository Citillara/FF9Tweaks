using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.IO;
using UnityModManagerNet;
using UnityEngine;
using FF9Tweaks.Tools;
using static UnityModManagerNet.UnityModManager.ModEntry;
using System.Collections;
using FF9Tweaks.Patches;

namespace FF9Tweaks
{
    public static class Main
    {
        internal static Settings Settings;

        internal static ModLogger Logger;

        internal static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Settings = Settings.Load<Settings>(modEntry);

            Logger = modEntry.Logger;

            mod = modEntry;
            mod.OnGUI = OnGUI;
            mod.OnSaveGUI = OnSaveGUI;


            loopRun = true;

            mre.Reset();
            /*Proxy_PARTY_DATA proxy = new Proxy_PARTY_DATA();
            proxy.Copy(FF9StateSystem.Common.FF9.party);
            FF9StateSystem.Common.FF9.party = proxy;*/


            return true;
        }

        static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.OnGui(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        static ManualResetEvent mre = new ManualResetEvent(false);
        static bool loopRun = false;

        static void Loop()
        {
            while (loopRun)
            {
                mre.WaitOne(20000);
            }
        }


        public static void Test()
        {
            int i = 0;
            foreach (PLAYER p in FF9StateSystem.Common.FF9.player)
            {
                Logger.Log($"player{i}:{p.name}");
                i++;
            }

            i = 0;
            foreach (PLAYER p in FF9StateSystem.Common.FF9.party.member)
            {
                Logger.Log($"party{i}:{p.name}");
                i++;
            }
        }


        public static void Test2()
        {
        }

        
    }

    

    /*[HarmonyPatch(typeof(DialogManager), "SignalProcess")]
    class InstantTextPatch
    {
        static bool Prefix(UILabel label, string fullText, ref IEnumerator __result)
        {
            
            label.text = fullText;
            __result = null; 
            return false;
        }
    }
    */

    /*[HarmonyPatch(typeof(TypewriterEffect), "Update")]
    class TypewriterEffect_Update
    {
        static bool Prefix(ref TypewriterEffect __instance)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = typeof(TypewriterEffect).GetField("mFullText", bindFlags);

            field.SetValue(__instance, "");

            return true;
        }
    }*/


    //[HarmonyPatch(typeof(DialogAnimator), "StartShowDialog")]
    //class DialogAnimator_StartShowDialog
    //{
    //    static bool Prefix(ref DialogAnimator __instance)
    //    {
    //        BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    //        FieldInfo field = typeof(DialogAnimator).GetField("showWithoutAnimation", bindFlags);
    //
    //        field.SetValue(__instance, true);
    //
    //
    //        /*BindingFlags bindFlags2 = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    //        FieldInfo field2 = typeof(DialogAnimator).GetField("showWithoutAnimation", bindFlags2);
    //
    //        field2.SetValue(__instance, 1f);*/
    //
    //        __instance.Pause = false;
    //
    //        return true;
    //    }
    //}
    //
    //[HarmonyPatch(typeof(DialogAnimator), "StartHideDialog")]
    //class DialogAnimator_StartHideDialog
    //{
    //    static bool Prefix(ref DialogAnimator __instance)
    //    {
    //        BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    //        FieldInfo field = typeof(DialogAnimator).GetField("progress", bindFlags);
    //
    //        field.SetValue(__instance, 1f);
    //
    //        __instance.Pause = false;
    //
    //        return true;
    //    }
    //}
    //
    //[HarmonyPatch(typeof(DialogAnimator), "ShowAnimationTime", MethodType.Getter)]
    //public static class DialogAnimator_ShowAnimationTime
    //{
    //    static bool Prefix(ref float __result)
    //    {
    //        __result = 0f; // Set the result to 0
    //        return false; // Skip the original method
    //    }
    //}
}
