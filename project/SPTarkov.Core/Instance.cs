using UnityEngine;
using SPTarkov.Core.Patches;
using SPTarkov.Common.Utils.Patching;

namespace SPTarkov.Core
{
	public class Instance : MonoBehaviour
	{
		private void Start()
		{
            Debug.LogError("SPTarkov.Core: Loaded");

            PatcherUtil.Patch<BattleEyePatch>();
            PatcherUtil.Patch<SslCertificatePatch>();
            PatcherUtil.Patch<UnityWebRequestPatch>();
            PatcherUtil.Patch<NotificationSslPatch>();
        }
	}
}
