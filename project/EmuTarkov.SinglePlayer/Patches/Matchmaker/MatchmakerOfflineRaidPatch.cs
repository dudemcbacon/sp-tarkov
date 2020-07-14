using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils;
using EmuTarkov.SinglePlayer.Utils.DefaultSettings;

namespace EmuTarkov.SinglePlayer.Patches.Matchmaker
{
    class MatchmakerOfflineRaidPatch : AbstractPatch
    {
        public static void Postfix(UpdatableToggle ____offlineModeToggle, UpdatableToggle ____botsEnabledToggle,
            TMPDropDownBox ____aiAmountDropdown, TMPDropDownBox ____aiDifficultyDropdown, UpdatableToggle ____enableBosses,
            UpdatableToggle ____scavWars, UpdatableToggle ____taggedAndCursed)
        {
            ____offlineModeToggle.isOn = true;
            ____offlineModeToggle.gameObject.SetActive(false);
            ____botsEnabledToggle.isOn = true;

            DefaultRaidSettings defaultRaidSettings = Settings.DefaultRaidSettings;
            if (defaultRaidSettings != null) {
                ____aiAmountDropdown.UpdateValue((int)defaultRaidSettings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)defaultRaidSettings.AiDifficulty, false);
                ____enableBosses.isOn = defaultRaidSettings.BossEnabled;
                ____scavWars.isOn = defaultRaidSettings.ScavWars;
                ____taggedAndCursed.isOn = defaultRaidSettings.TaggedAndCursed;
            }
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(MatchmakerOfflineRaid).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}