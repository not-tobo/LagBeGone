using MelonLoader;
using System.Collections;

namespace LagBeGone
{
    public static class ModInfo
    {
        public const string Name = "LagBeGone";
        public const string Author = "Topi#1337";
        public const string Version = "0.1.0";
        public const string DownloadLink = "";
    }

    public class main : MelonMod
    {
        public static readonly string SettingsCategory = "LagBeGone";
        public static readonly string entrySizeLimit = "sizeLimit";
        public static readonly string entryRateLimit = "rateLimit";

        public static bool sizeLimit
        {
            get { return MelonPreferences.GetEntryValue<bool>(SettingsCategory, entrySizeLimit); }
            set { MelonPreferences.SetEntryValue<bool>(SettingsCategory, entrySizeLimit, value); }
        }

        public static bool rateLimit
        {
            get { return MelonPreferences.GetEntryValue<bool>(SettingsCategory, entryRateLimit); }
            set { MelonPreferences.SetEntryValue<bool>(SettingsCategory, entryRateLimit, value); }
        }

        public override void OnApplicationStart()
        {
            MelonCoroutines.Start(StartUiManagerInitIEnumerator());
            MelonPreferences.CreateEntry<bool>(SettingsCategory, entrySizeLimit, true, "Limit Event 9 Size [best Methode]");
            MelonPreferences.CreateEntry<bool>(SettingsCategory, entryRateLimit, true, "Limit Event 9 to 1 per second per user");
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
    }
}