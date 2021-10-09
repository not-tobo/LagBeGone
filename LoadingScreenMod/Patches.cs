using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HarmonyPatches
{
    public class HPatch
    {
        public static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Topi Patches");
        public string Name;
        public MethodBase TargetMethod;
        public HarmonyMethod Prefix;
        public HarmonyMethod Postfix;

        public HPatch(string name, MethodBase target, HarmonyMethod prefix, HarmonyMethod postfix)
        {
            Name = name;
            TargetMethod = target;
            Prefix = prefix;
            Postfix = postfix;
        }

        public HPatch(string name, HarmonyMethod prefix, MethodBase target)
        {
            Name = name;
            TargetMethod = target;
            Prefix = prefix;
            Postfix = null;
        }

        public HPatch(string name, MethodBase target, HarmonyMethod postfix)
        {
            Name = name;
            TargetMethod = target;
            Prefix = null;
            Postfix = postfix;
        }

        public void Patch()
        {
            harmony.Patch(TargetMethod, Prefix, Postfix);
        }
    }

    public class Patching
    {
        public static bool patchesDone = false;
        public static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Topi Patches");

        public static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Patching).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
        }

        public static void Initpatch()
        {
            List<HPatch> patches = new List<HPatch>()
            {
                new HPatch("OnEventClient1", GetPatch(nameof(OnEvent)), AccessTools.Method(typeof(LoadBalancingClient), "OnEvent")),
            };

            int patchcount = 0;
            int patchcountsuccess = 0;

            MelonLogger.Msg(ConsoleColor.DarkYellow, "Appliying Patches...");
            foreach (HPatch patch in patches)
            {
                patchcount++;
                try
                {
                    patch.Patch();
                    patchcountsuccess++;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("Error during Patch of " + patch.Name);
                    MelonLogger.Error(ex.ToString());
                }
            }
            patchesDone = true;
            MelonLogger.Msg(ConsoleColor.DarkGreen, patchcountsuccess + "/" + patchcount + " Patches Applied!");
        }

        public static List<string> blacklistedUsers = new List<string>();

        private static bool OnEvent(ref EventData __0)
        {
            if (__0.Code == 9)
            {
                int playerID = __0.Sender;
                string eventSender = PlayerManagerExtension.PlayerManagerExtension.GetPlayer(PlayerManagerExtension.PlayerManagerExtension.PlayerManager, playerID).field_Private_VRCPlayerApi_0.displayName;

                if (LagBeGone.main.sizeLimit)
                {
                    byte[] syncData = (byte[])Serialize.Serialize.FromIL2CPPToManaged<object>(__0.CustomData);
                    if (syncData.Length > 200)
                    {
                        MelonLogger.Msg("Blocked Event 9 because Size to big (Size: " + syncData.Length + ") from: " + eventSender);
                        return false;
                    }
                }

                if (LagBeGone.main.rateLimit)
                {
                    if (!blacklistedUsers.Contains(eventSender))
                    {
                        blacklistedUsers.Add(eventSender);
                        Delay(1f, delegate
                        {
                            blacklistedUsers.Remove(eventSender);
                        });
                        return true;
                    }
                    MelonLogger.Msg("Rate limited Event 9 from: " + eventSender);
                    return false;
                }
            }
            return true;
        }

        internal static void Delay(float del, Action action)
        {
            MelonCoroutines.Start(DelayFunc(del, action));
        }

        private static IEnumerator DelayFunc(float del, Action action)
        {
            yield return new WaitForSeconds(del);
            action.Invoke();
            yield break;
        }
    }
}