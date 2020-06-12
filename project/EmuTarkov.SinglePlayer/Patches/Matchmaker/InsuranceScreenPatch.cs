using System.Reflection;
using EmuTarkov.Common.Utils.Patching;
using NextScreenShowAction = GClass1110;

namespace EmuTarkov.SinglePlayer.Patches.Matchmaker
{
    class InsuranceScreenPatch : AbstractPatch
    {
        public static void Prefix(bool local, GStruct73 weatherSettings, GStruct177 botsSettings, GStruct74 wavesSettings)
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