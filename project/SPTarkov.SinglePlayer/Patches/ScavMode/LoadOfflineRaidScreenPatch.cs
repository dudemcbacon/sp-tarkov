using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils.Reflection;
using MenuController = GClass1137;
using WeatherSettings = GStruct90;
using BotsSettings = GStruct223;
using WavesSettings = GStruct91;

namespace SPTarkov.SinglePlayer.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;

    public class LoadOfflineRaidScreenPatch : GenericPatch<LoadOfflineRaidScreenPatch>
    {
        private static readonly string kMenuControllerInnerType = "Class787";
        private static readonly string kMainControllerFieldName = "gclass1137_0";
        private static readonly string kBotsSettingsFieldName = "gstruct223_0";
        private static readonly string kWeatherSettingsFieldName = "gstruct90_0";
        private static readonly string kWavesSettingsFieldName = "gstruct91_0";

        private static readonly string kTargetMethodName = "method_2";
        private static readonly string kLoadReadyScreenMethodName = "method_36";
        private static readonly string kReadyMethodName = "method_54";

        public LoadOfflineRaidScreenPatch() : base(transpiler: nameof(PatchTranspiler)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.Name == kMenuControllerInnerType)
                .GetMethod(kTargetMethodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);

            int index = 29;

            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreenPatch), "LoadOfflineRaidScreenForScav"));
            codes[index].opcode = OpCodes.Nop;
            codes[index + 1] = callCode;
            codes.RemoveAt(index + 2);
            return codes.AsEnumerable();
        }

        private static MenuController GetMenuController()
        {
            return PrivateValueAccessor.GetPrivateFieldValue(typeof(MainApplication), 
                kMainControllerFieldName, ClientAppUtils.GetMainApp()) as MenuController;
        }


        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            MenuController menuController = GetMenuController();
            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            SetMenuControllerFieldValue(menuController, "bool_0", local);

            SetMenuControllerFieldValue(menuController, kBotsSettingsFieldName, botsSettings);
            SetMenuControllerFieldValue(menuController, kWavesSettingsFieldName, wavesSettings);
            SetMenuControllerFieldValue(menuController, kWeatherSettingsFieldName, weatherSettings);

            typeof(MenuController).GetMethod(kLoadReadyScreenMethodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menuController, null);
        }

        public static void LoadOfflineRaidScreenForScav()
        {
            MenuController menuController = GetMenuController();

            MatchmakerOfflineRaid.GClass1902 gclass = new MatchmakerOfflineRaid.GClass1902();
            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), (object)menuController, kReadyMethodName);
            gclass.ShowScreen(EScreenState.Queued);
        }

        private static void SetMenuControllerFieldValue(MenuController instance, string fieldName, object value)
        {
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), fieldName, instance, value);
        }
    }
}
