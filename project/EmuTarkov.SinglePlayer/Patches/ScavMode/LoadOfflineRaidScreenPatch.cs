using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Reflection;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using MenuController = GClass1099;

namespace EmuTarkov.SinglePlayer.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, GStruct73, GStruct178, GStruct74>;

    public class LoadOfflineRaidScreenPatch : GenericPatch<LoadOfflineRaidScreenPatch>
    {
        public LoadOfflineRaidScreenPatch() : base(transpiler: nameof(PatchTranspiler)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.Name == "Class761")
                .GetMethod("method_2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);

            int index = 29;

            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreenPatch), "LoadOfflineRaidScreenForScav"));
            codes[index].opcode = OpCodes.Nop;
            codes[index + 1] = callCode;
            codes.RemoveAt(index+2);
            return codes.AsEnumerable();
        }

        private static MenuController GetMenuController()
        {
            return PrivateValueAccessor.GetPrivateFieldValue(typeof(MainApplication),
                "gclass1099_0", ClientAppUtils.GetMainApp()) as MenuController;
        }


        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, GStruct73 weatherSettings, GStruct178 botsSettings, GStruct74 wavesSettings)
        {
            MenuController menuController = GetMenuController();
            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }
            
            SetMenuControllerFieldValue(menuController, "bool_0", local);

            SetMenuControllerFieldValue(menuController, "gstruct178_0", botsSettings);
            SetMenuControllerFieldValue(menuController, "gstruct74_0", wavesSettings);
            SetMenuControllerFieldValue(menuController, "gstruct73_0", weatherSettings);
            
            typeof(MenuController).GetMethod("method_36", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menuController, null);
        }

        public static void LoadOfflineRaidScreenForScav()
        {
            MenuController menuController = GetMenuController();

            MatchmakerOfflineRaid.GClass1850 gclass = new MatchmakerOfflineRaid.GClass1850();
            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), (object)menuController, "method_54");
            gclass.ShowScreen(EScreenState.Queued);
        }

        private static void SetMenuControllerFieldValue(MenuController instance, string fieldName, object value) {
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), fieldName, instance, value);
        }
    }
}
