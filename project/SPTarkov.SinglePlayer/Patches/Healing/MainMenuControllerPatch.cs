using System.Reflection;
using SPTarkov.Common.Utils.Patching;
using MainMenuController = GClass1137;
using IHealthController = GInterface157;

namespace SPTarkov.SinglePlayer.Patches.Healing
{
    class MainMenuControllerPatch : GenericPatch<MainMenuControllerPatch>
    {
        static MainMenuControllerPatch()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        public MainMenuControllerPatch() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utils.Player.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
