using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils.Reflection;

namespace SPTarkov.SinglePlayer.Patches.ScavMode
{
    public class ScavSpawnPointPatch : GenericPatch<ScavSpawnPointPatch>
    {
        public ScavSpawnPointPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.TargetAssembly.GetTypes().FirstOrDefault(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Select(y => y.Name).Contains("SelectFarthestFromOtherPlayers")).GetNestedTypes(BindingFlags.NonPublic).FirstOrDefault(
                x => x.GetField("infiltrationZone") != null).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).
                Single(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(SpawnArea.SpawnAreaSettings));
        }

        static bool PatchPrefix(ref bool __result, object __instance)
        {
            EPlayerSide playerSide = (EPlayerSide)PrivateValueAccessor.GetPrivateFieldValue(__instance.GetType(), "side", __instance);
            if (playerSide == EPlayerSide.Savage)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
