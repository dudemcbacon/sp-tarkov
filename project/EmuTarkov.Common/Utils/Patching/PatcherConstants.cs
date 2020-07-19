using System;
using System.Linq;
using System.Reflection;
using EFT;

namespace EmuTarkov.Common.Utils.Patching
{
    public static class PatcherConstants
    {
        public static Assembly TargetAssembly = typeof(AbstractGame).Assembly;
        public static Type MainApplicationType = TargetAssembly.GetTypes().Single(x => x.Name == "MainApplication");
        public static Type LocalGameType = TargetAssembly.GetTypes().Single(x => x.Name == "LocalGame");
        public static Type MatchmakerOfflineRaidType = TargetAssembly.GetTypes().Single(x => x.Name == "MatchmakerOfflineRaid");
        public static Type BaseLocalGameType = TargetAssembly.GetTypes().Single(x => x.Name == "BaseLocalGame`1").MakeGenericType(typeof(GamePlayerOwner));
        public static Type MenuControllerType = TargetAssembly.GetTypes().Single(x => x.GetProperty("QuestController") != null);

        public static Type BackendInterfaceType = TargetAssembly.GetTypes().Single(
            x => x.GetMethods().Select(y => y.Name).Contains("CreateClientSession") && x.IsInterface);
        public static Type SessionInterfaceType = TargetAssembly.GetTypes().Single(
            x => x.GetMethods().Select(y => y.Name).Contains("GetPhpSessionId") && x.IsInterface);

        public static Type ExfilPointManagerType = TargetAssembly.GetTypes().Single(x => x.GetMethod("InitAllExfiltrationPoints") != null);
        public static Type ProfileInfoType = TargetAssembly.GetTypes().Single(x => x.GetMethod("GetExperience") != null);

    }
}
