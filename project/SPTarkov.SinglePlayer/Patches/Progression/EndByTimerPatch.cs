using EFT;
using SPTarkov.Common.Utils.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPTarkov.SinglePlayer.Patches.Progression
{
    /// <summary>
    /// Fixes exit status to 'MissingInAction' when the raid time ends. Default is Survived.
    /// </summary>
    class EndByTimerPatch : GenericPatch<EndByTimerPatch>
    {
        private static PropertyInfo _profileIdProperty;
        private static MethodInfo _stopRaidMethod;

        static EndByTimerPatch()
        {
            _profileIdProperty = PatcherConstants.LocalGameType
                .BaseType
                .GetProperty("ProfileId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("'ProfileId' property not found");

            // find method
            // protected void method_11(string profileId, ExitStatus exitStatus, string exitName, float delay = 0f)
            _stopRaidMethod = PatcherConstants.LocalGameType
                .BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .SingleOrDefault(IsStopRaidMethod)
                ?? throw new InvalidOperationException("Method not found");
        }

        private static bool IsStopRaidMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            if (parameters.Length != 4
             || parameters[0].ParameterType != typeof(string)
             || parameters[0].Name != "profileId"
             || parameters[1].ParameterType != typeof(ExitStatus)
             || parameters[1].Name != "exitStatus"
             || parameters[2].ParameterType != typeof(string)
             || parameters[2].Name != "exitName"
             || parameters[3].ParameterType != typeof(float)
             || parameters[3].Name != "delay")
                return false;
            return true;
        }

        public EndByTimerPatch() : base(prefix: nameof(PrefixPatch)) { }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.LocalGameType
                .BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single(x => x.Name.EndsWith("StopGame"));  // find explicit interface implementation
        }

        private static bool PrefixPatch(object __instance)
        {
            var profileId = _profileIdProperty.GetValue(__instance) as string;

            _stopRaidMethod.Invoke(__instance, new object[] { profileId, ExitStatus.MissingInAction, null, 0f });

            return false;
        }
    }
}
