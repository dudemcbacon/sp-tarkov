using System.Reflection;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils.Reflection;

namespace SPTarkov.SinglePlayer.Patches.RaidFix
{
    public class OnShellEjectEventPatch : GenericPatch<OnShellEjectEventPatch>
    {
        public OnShellEjectEventPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.FirearmControllerType.GetMethod("OnShellEjectEvent");
        }

        static bool PatchPrefix(object __instance)
        {
            object weaponController = PrivateValueAccessor.GetPrivateFieldValue(PatcherConstants.FirearmControllerType, PatcherConstants.WeaponControllerFieldName, __instance);
            return (weaponController.GetType().GetField("RemoveFromChamberResult").GetValue(weaponController) == null) ? false : true;
        }
    }
}
