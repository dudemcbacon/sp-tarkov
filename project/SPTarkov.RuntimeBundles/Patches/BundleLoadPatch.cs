using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Diz.DependencyManager;
using HarmonyLib;
using SPTarkov.Common.Utils.Patching;
using IBundleLock = GInterface224; //Property: IsLocked
using BindableState = GClass2037<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Diz.Jobs;
using System.Text.RegularExpressions;
using SPTarkov.RuntimeBundles.Utils;

namespace SPTarkov.RuntimeBundles.Patches
{
    public class BundleLoadPatch : GenericPatch<BundleLoadPatch>
    {
        private static readonly CertificateHandler _certificateHandler = new FakeCertificateHandler();

        public BundleLoadPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("Class1886"), "method_0");
            //return PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType).GetMethod("method_0");
        }

        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }


        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (!mi.Name.Equals("LoadFromFileAsync") || parameters.Length != 1 || parameters[0].Name != "path") ? false : true;
        }

        static bool PatchPrefix(object __instance, IBundleLock ___ginterface224_0, bool ___bool_0, string ___string_1, string ___string_0, Task __result)
        {
            if (___string_1.IndexOf("http") == -1)
            {
                return true;
            }

            __result = LoadBundleFromServer(__instance, ___ginterface224_0, ___bool_0, ___string_1, ___string_0);
            return false;
        }

        private static async Task LoadBundleFromServer(object __instance, IBundleLock bundleLock, bool shouldBeLoaded, string path, string keyWithoutExtension)
        {
            Traverse trav = Traverse.Create(__instance);
            EasyBundleHelper easyBundle = new EasyBundleHelper(__instance);

            bool cached = false;
            var bundleKey = Regex.Split(path, "bundle/", RegexOptions.IgnoreCase)[1];
            var cachePath = "Cache/StreamingAssets/windows/";

            if (File.Exists("Cache/StreamingAssets/windows/" + bundleKey))
            {
                cached = true;
                easyBundle.Path = cachePath + bundleKey;
                path = cachePath + bundleKey;

            }

            if (!cached && path.IndexOf("http") != -1)
            {
                using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(path))
                {
                    unityWebRequest.certificateHandler = _certificateHandler;
                    unityWebRequest.disposeCertificateHandlerOnDispose = false;
                    //unityWebRequest.timeout = 30000;
                    await unityWebRequest.SendWebRequest().Await();
                    if (!unityWebRequest.isNetworkError && !unityWebRequest.isHttpError)
                    {
                        var fileName = path.Split('/').ToList().Last();
                        var dirPath = Regex.Split(bundleKey, fileName, RegexOptions.IgnoreCase)[0];

                        if (!Directory.Exists(cachePath + dirPath))
                        {
                            Directory.CreateDirectory(cachePath + dirPath);

                        }
                        File.WriteAllBytes(cachePath + bundleKey, unityWebRequest.downloadHandler.data);
                        easyBundle.Path = cachePath + bundleKey;
                        path = cachePath + bundleKey;
                    }
                    else
                    {
                        Debug.Log("cant load " + path + " because of error " + unityWebRequest.error);
                    }
                }
            }
            await easyBundle.LoadingCoroutine();

        }
    }

    internal class FakeCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
