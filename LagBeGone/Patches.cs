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

            LagBeGone.main.TopiLogger("Appliying Patches...", ConsoleColor.DarkYellow, "patch");
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
                    LagBeGone.main.TopiLogger("Error during Patch of " + patch.Name, ConsoleColor.Red, "patch");
                    LagBeGone.main.TopiLogger(ex.ToString(), ConsoleColor.Red, "patch");
                }
            }
            patchesDone = true;
            LagBeGone.main.TopiLogger(patchcountsuccess + "/" + patchcount + " Patches Applied!", ConsoleColor.Green, "patch");
        }

        public static List<string> blacklistedUsers = new List<string>();
        public static int countEvents = 0;
        public static int countLagger = 0;
        public static bool laggerLogged = false;
        public static bool blocked = false;

        private static bool OnEvent(ref EventData __0)
        {
            if (__0.Code == 9 || __0.Code == 6)
            {
                int playerID = __0.Sender;;

                if (LagBeGone.main.spamProtection)
                {
                    if (blocked)
                    {
                        countEvents++;
                        return false;
                    }  
                    if (countEvents >= LagBeGone.main.spamProtectionValue)
                    {
                        if(!blocked)
                        {
                            LagBeGone.main.TopiLogger("Blocking Event 6/9 for 10 seconds", ConsoleColor.Red, "spam");
                            blocked = true;
                            Delay(10f, delegate
                            {
                                LagBeGone.main.TopiLogger("Unblocked Event 6/ 9 [ " + countEvents + " blocked]", ConsoleColor.Green, "spam");
                                countEvents = 0;
                                blocked = false;
                            });
                        }
                        return false;
                    }
                    countEvents++;
                    //LagBeGone.main.TopiLogger("[Debug] Event 9 Counter: " + countEvent9, ConsoleColor.DarkGray, "spam");
                    Delay(1f, delegate
                    {
                        countEvents = 0;
                    });
                }
                if (LagBeGone.main.sizeLimit && __0.Code == 9)
                {
                    byte[] syncData = (byte[])Serialize.Serialize.FromIL2CPPToManaged<object>(__0.CustomData);
                    if (syncData.Length >= LagBeGone.main.sizeLimitValue)
                    {
                        if (countLagger > 50 && !laggerLogged)
                        {
                            laggerLogged = true;
                            LagBeGone.main.TopiLogger(PlayerManagerExtension.PlayerManagerExtension.GetPlayer(PlayerManagerExtension.PlayerManagerExtension.PlayerManager, playerID).field_Private_VRCPlayerApi_0.displayName + " is using a Event 9 Lagger!", ConsoleColor.Red, "size");
                            Delay(10f, delegate
                            {
                                laggerLogged = false;
                            });
                        }
                        countLagger++;
                        Delay(1f, delegate
                        {
                            countLagger = 0;
                        });
                        //LagBeGone.main.TopiLogger("[Debug] Blocked Event 9 because Size to big (Size: " + syncData.Length, ConsoleColor.Gray, "size");
                        return false;
                    }
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