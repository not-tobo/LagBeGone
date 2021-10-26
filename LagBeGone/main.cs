﻿using MelonLoader;
using System;
using System.Collections;

namespace LagBeGone
{
    public static class ModInfo
    {
        public const string Name = "LagBeGone";
        public const string Author = "Topi#1337";
        public const string Version = "0.1.1";
        public const string DownloadLink = "";
    }

    public class main : MelonMod
    {
        public static readonly string SettingsCategory = "LagBeGone";
        public static readonly string entrySizeLimit = "sizeLimit";
        public static readonly string entrySpamProtection = "spamProtection";
        public static readonly string entryFullBlock = "fullBlock";

        public static bool sizeLimit
        {
            get { return MelonPreferences.GetEntryValue<bool>(SettingsCategory, entrySizeLimit); }
            set { MelonPreferences.SetEntryValue<bool>(SettingsCategory, entrySizeLimit, value); }
        }

        public static bool spamProtection
        {
            get { return MelonPreferences.GetEntryValue<bool>(SettingsCategory, entrySizeLimit); }
            set { MelonPreferences.SetEntryValue<bool>(SettingsCategory, entrySizeLimit, value); }
        }

        public static bool fullBlock
        {
            get { return MelonPreferences.GetEntryValue<bool>(SettingsCategory, entryFullBlock); }
            set { MelonPreferences.SetEntryValue<bool>(SettingsCategory, entryFullBlock, value); }
        }

        public override void OnApplicationStart()
        {
            MelonCoroutines.Start(StartUiManagerInitIEnumerator());
            MelonPreferences.CreateEntry<bool>(SettingsCategory, entrySizeLimit, true, "Event 9 Size Limiter [good against Event 9 Lagger]");
            MelonPreferences.CreateEntry<bool>(SettingsCategory, entrySpamProtection, true, "Event 9 Spam Protection [good against Photon Bots]");
            MelonPreferences.CreateEntry<bool>(SettingsCategory, entryFullBlock, true, "Block all Event 9 [idk, dont use it. It breaks Avatar 3.0 sync]");
        }

        private IEnumerator StartUiManagerInitIEnumerator()
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null)
                yield return null;

            VRChat_OnUiManagerInit();
        }

        public static HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Topi Patches");

        public static void VRChat_OnUiManagerInit()
        {
            HarmonyPatches.Patching.Initpatch();
        }

        public override void OnPreferencesSaved()
        {
        }

        public static void TopiLogger(string message, ConsoleColor color = ConsoleColor.White, string type = "none")
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write("[");
            System.Console.ForegroundColor = ConsoleColor.Blue;
            string SystemTime = DateTime.Now.ToString("h:mm:ss.ms");
            System.Console.Write(SystemTime);
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write("] [");
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.Write("LagBeGone");
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write("] ");
            switch (type)
            {
                case "spam":
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("[");
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                    System.Console.Write("Spam Protection");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("] ");
                    break;
                case "size":
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("[");
                    System.Console.ForegroundColor = ConsoleColor.Magenta;
                    System.Console.Write("Size Limiter");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("] ");
                    break;
                case "patch":
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("[");
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.Write("Patches");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Write("] ");
                    break;
                default:
                    System.Console.Write(" ");
                    break;
            }
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
        }
    }
}