using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using SPTarkov.Common.Utils.Patching;

namespace SPTarkov.SinglePlayer.Patches.Matchmaker
{
    class MatchMakerSelectionLocationScreenPatch : AbstractPatch
    {
        public static void Postfix(UIButtonSpawner ____readyButton)
        {
            ____readyButton.SpawnedObject.gameObject.SetActive(false);
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}