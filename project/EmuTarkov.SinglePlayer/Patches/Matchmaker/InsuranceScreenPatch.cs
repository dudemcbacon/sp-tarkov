using System;
using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using EmuTarkov.Common.Utils.Patching;
using UnityEngine;
using NextScreenShowAction = GClass1108;

namespace EmuTarkov.SinglePlayer.Patches.Matchmaker
{
    class InsuranceScreenPatch : AbstractPatch
    {
        public static void Prefix(ref bool local, GStruct77 weatherSettings, GStruct196 botsSettings, GStruct78 wavesSettings)
        {
            local = false;
        }
        public static void Postfix(ref bool ___bool_0)
        {
            ___bool_0 = true;
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(NextScreenShowAction).GetMethod("method_53", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}