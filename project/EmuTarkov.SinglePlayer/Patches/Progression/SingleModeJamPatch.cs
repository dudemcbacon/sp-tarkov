using System.Linq;
using System.Reflection;
using EFT;
using EmuTarkov.Common.Utils.Patching;
using HarmonyLib;
using System;
using EFT.InventoryLogic;

namespace EmuTarkov.SinglePlayer.Patches.Progression
{
    public class SingleModeJamPatch : AbstractPatch
    {
        private static MethodInfo _onFireEventMethod;

        public override MethodInfo TargetMethod()
        {
            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            var targetMethod = targetType.GetMethod("PrepareShot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            _onFireEventMethod = targetType.GetMethod("OnFireEvent", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return targetMethod;
        }

        private static bool IsTargetType(Type type)
        {
            if (type.DeclaringType == null
                || type.DeclaringType.DeclaringType == null
                || type.DeclaringType.Name != nameof(Player.FirearmController)
                || type.DeclaringType.DeclaringType.Name != nameof(Player))
            {
                return false;
            }

            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (!methods.Any(x => x.Name == "PrepareShot"))
                return false;

            return true;
        }

        public static void Postfix(object __instance, Weapon ___weapon_0)
        {
            if (___weapon_0.MalfunctionState != Weapon.EMalfunctionState.Jam)
                return;

            _onFireEventMethod.Invoke(__instance, new object[] { });
        }
    }
}
