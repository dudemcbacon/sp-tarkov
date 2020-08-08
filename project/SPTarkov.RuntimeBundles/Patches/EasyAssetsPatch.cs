using System;
using System.Reflection;
using System.Threading.Tasks;
using Diz.Jobs;
using Diz.Resources;
using HarmonyLib;
using JetBrains.Annotations;
using SPTarkov.Common.Utils.Patching;
using UnityEngine;
using IEasyBundle = GInterface223; //Property: SameNameAsset 
using IBundleLock = GInterface224; //Property: IsLocked
using BundleLock = GClass2052; //Property: MaxConcurrentOperations
using DependencyGraph = GClass2053<GInterface223>; // Method: GetDefaultNode()
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using SPTarkov.RuntimeBundles.Utils;
using UnityEngine.Networking;

namespace SPTarkov.RuntimeBundles.Patches
{
    public class EasyAssetsPatch : GenericPatch<EasyAssetsPatch>
    {
        private static Type easyBundleType;
        private static string easyBundleTypeName;
        private static string bundlesFieldName;

        public EasyAssetsPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            // return AccessTools.Method("Diz.Resources.EasyAssets:Init");

            easyBundleType = PatcherConstants.TargetAssembly.GetTypes()
                .Single(type => type.IsClass && type.GetProperty("SameNameAsset") != null);
            bundlesFieldName = easyBundleType.Name.ToLower() + "_0";

            Type targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            return AccessTools.GetDeclaredMethods(targetType).Single(IsTargetMethod);
        }

        private static bool IsTargetType(Type type)
        {
            //TODO: Development needs, to be deleted later
            if (type == typeof(EasyAssets))
            {
                Debugger.Break();
            }
            FieldInfo[] fields = type.GetFields();

            if (fields.Length > 2)
                return false;

            if (!fields.Any(x => x.Name == "Manifest"))
                return false;

            return type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) != null;

        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 5 || parameters[0].Name != "bundleLock" || parameters[1].Name != "defaultKey" || parameters[4].Name != "shouldExclude") ? false : true;
        }

        static bool PatchPrefix(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude, ref Task __result)
        {
            __result = Init(__instance, bundleLock, defaultKey, rootPath, platformName, shouldExclude);
            return false;
        }

        public static async Task Init(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude)
        {
            Traverse traverse = Traverse.Create(__instance);
            string path = rootPath.Replace("file:///", "").Replace("file://", "") + "/" + platformName + "/";
            
            AssetBundleCreateRequest manifestLoading = AssetBundle.LoadFromFileAsync(path + platformName);
            await manifestLoading.Await();
            AssetBundle assetBundle = manifestLoading.assetBundle;
            AssetBundleRequest assetLoading = assetBundle.LoadAllAssetsAsync();

            await assetLoading.Await();
            traverse.Field<AssetBundleManifest>("Manifest").Value = (AssetBundleManifest)assetLoading.allAssets[0];
            AssetBundleManifest manifest = traverse.Field<AssetBundleManifest>("Manifest").Value;

            //Add ModManifest
            List<string> result = manifest.GetAllAssetBundles().ToList<string>();
            List<string> resourcesModbundles = new List<string>();


            foreach (KeyValuePair<string, BundleInfo> kvp in Settings.bundles)
            {
                resourcesModbundles.Add(kvp.Key);
            }

            string[] bundleNames = result.Union(resourcesModbundles).ToList<string>().ToArray<string>();

            //string[] bundleNames = manifest.GetAllAssetBundles();
            traverse.Field(bundlesFieldName).SetValue(Array.CreateInstance(easyBundleType, bundleNames.Length));
            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            IEasyBundle[] bundles = traverse.Field(bundlesFieldName).GetValue<IEasyBundle[]>();
            var startTime = Time.time;
            for (int i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(easyBundleType, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            var endTime = Time.time;
            UnityEngine.Debug.LogError(endTime - startTime);
            traverse.Field(bundlesFieldName).SetValue(bundles);
            traverse.Property<DependencyGraph>("System").Value = new DependencyGraph(bundles, defaultKey, shouldExclude);
        }
    }


}
