using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils;
using static EFT.Player;

namespace EmuTarkov.SinglePlayer.Patches.Progression
{
    public class SingleModeJamPatch : AbstractPatch
    {
        private static MethodInfo _onFireEventMethod;

        public override MethodInfo TargetMethod()
        {
            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            _onFireEventMethod = targetType.GetMethod("OnFireEvent", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return targetType.GetMethod("PrepareShot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
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
            {
                return false;
            }

            return true;
        }

        public static void Postfix(object __instance, Weapon ___weapon_0, FirearmsAnimator ___firearmsAnimator_0, FirearmController ___firearmController_0)
        {
            if (!Settings.WeaponDurabilityEnabled || ___weapon_0.MalfunctionState != Weapon.EMalfunctionState.Jam)
            {
                return;
            }

            _onFireEventMethod.Invoke(__instance, new object[] { });

            ___firearmsAnimator_0.Animator.Play("JAM", 1, 0f);
            ___firearmController_0.EmitEvents();
        }
    }
}
