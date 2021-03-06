﻿using System.Reflection;
using UnityEngine;
using EFT.UI;
using EFT.UI.Matchmaker;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using SPTarkov.SinglePlayer.Utils.DefaultSettings;

namespace SPTarkov.SinglePlayer.Patches.Matchmaker
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

            if (defaultRaidSettings != null)
            {
                ____aiAmountDropdown.UpdateValue((int)defaultRaidSettings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)defaultRaidSettings.AiDifficulty, false);
                ____enableBosses.isOn = defaultRaidSettings.BossEnabled;
                ____scavWars.isOn = defaultRaidSettings.ScavWars;
                ____taggedAndCursed.isOn = defaultRaidSettings.TaggedAndCursed;
            }

            var warningPanel = GameObject.Find("Warning Panel");
            UnityEngine.Object.Destroy(warningPanel);
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(MatchmakerOfflineRaid).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}