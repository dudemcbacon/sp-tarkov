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
            return PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(IsTargetMethod);
        }

        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }


        private static bool IsTargetMethod(MethodInfo method)
        {
            return method.GetParameters().Length == 0 && method.ReturnType == typeof(Task);
        }

        static bool PatchPrefix(object __instance,string ___string_1, ref Task __result)
        {
            // ___string_1 == path
            if (___string_1.IndexOf("http") == -1)
            {
                return true;
            }

            __result = LoadBundleFromServer(__instance);
            return false;
        }

        private static async Task LoadBundleFromServer(object __instance)
        {
            EasyBundleHelper easyBundle = new EasyBundleHelper(__instance);
            var path = easyBundle.Path;
            var bundleKey = Regex.Split(path, "bundle/", RegexOptions.IgnoreCase)[1];
            var cachePath = Settings.cachePach;

            if (path.IndexOf("http") != -1)
            {
                using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(path))
                {
                    unityWebRequest.certificateHandler = _certificateHandler;
                    unityWebRequest.disposeCertificateHandlerOnDispose = false;
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
