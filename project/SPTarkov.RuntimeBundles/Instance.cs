using SPTarkov.Common.Utils.Patching;
using SPTarkov.RuntimeBundles.Patches;
using SPTarkov.RuntimeBundles.Utils;
using UnityEngine;


namespace SPTarkov.RuntimeBundles
{
    public class Instance : MonoBehaviour
    {
        private void Start()
		{
            Debug.LogError("SPTarkov.RuntimeBundles: Loaded");

            new Settings(null, Config.BackendUrl);

            PatcherUtil.Patch<EasyAssetsPatch>();
            PatcherUtil.Patch<EasyBundlePatch>();
            PatcherUtil.Patch<BundleLoadPatch>();
        }
    }
}
