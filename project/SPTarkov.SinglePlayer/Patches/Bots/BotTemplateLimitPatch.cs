using System.Collections.Generic;
using System.Reflection;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using WaveInfo = GClass855;
using BotsPresets = GClass296;

namespace SPTarkov.SinglePlayer.Patches.Bots
{
    public class BotTemplateLimitPatch : AbstractPatch
    {
        static BotTemplateLimitPatch()
        {
            // compile-time checks
            _ = nameof(BotsPresets.CreateProfile);
            _ = nameof(WaveInfo.Difficulty);
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(BotsPresets).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void Postfix(List<WaveInfo> __result, List<WaveInfo> wavesProfiles, List<WaveInfo> delayed)
        {
            /*
                In short this method sums Limits by grouping wavesPropfiles collection by Role and Difficulty
                then in each group sets Limit to 30, the remainder is stored in "delayed" collection.
                So we change Limit of each group.
                Clear delayed waves, we don't need them if we have enough loaded profiles and in method_2 it creates a lot of garbage.
            */

            delayed?.Clear();
            
            foreach (WaveInfo wave in __result)
            {
				wave.Limit = Settings.Limits[wave.Role];
            }
        }
    }
}
