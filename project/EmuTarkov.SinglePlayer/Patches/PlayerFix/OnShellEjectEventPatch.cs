using EmuTarkov.Common.Utils.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFT;
using EmuTarkov.SinglePlayer.Utils.Reflection;

namespace EmuTarkov.SinglePlayer.Patches.PlayerFix
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
            if (weaponController.GetType().GetField("RemoveFromChamberResult").GetValue(weaponController) == null)
            {
                return false;
            }
            return true;
        }
    }
}
