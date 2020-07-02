using System;
using System.Reflection;
using BehaviourMachine;
using EFT.UI;
using EFT.UI.Matchmaker;
using EmuTarkov.Common.Utils.Patching;

namespace EmuTarkov.SinglePlayer.Patches.Matchmaker
{
    class MatchMakerSideSelectionScreenPatch : AbstractPatch
    {
        public static void Postfix(LocalizedText ____savageBlockMessage)
        {
            ____savageBlockMessage.LocalizationKey = "DISABLED";
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(MatchMakerSideSelectionScreen).GetMethod("method_7", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}