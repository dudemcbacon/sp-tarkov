using System;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using SPTarkov.Common.Utils.HTTP;
using Newtonsoft.Json.Linq;

namespace SPTarkov.RuntimeBundles.Utils
{
    public class Settings
    {
		private static string Session;
		private static string BackendUrl;
        public readonly static Dictionary<string, BundleInfo> bundles = new Dictionary<string, BundleInfo>();

		public Settings(string session, string backendUrl)
		{
            Session = session;
            BackendUrl = backendUrl;

            Init(); 
        }

		private void Init()
		{
			string json = new Request(Session, BackendUrl).GetJson("/singleplayer/bundles/");
            if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("SPTarkov.RuntimeBundles: Bundles data is Null, using fallback");
				return;
			}

            var jArray = JArray.Parse(json);
            foreach (var jObj in jArray)
            {
                BundleInfo bundle;
                if (!bundles.TryGetValue(jObj["key"].ToString(), out bundle))
                {
                    bundle = new BundleInfo(jObj["key"].ToString(), jObj["path"].ToString(), jObj["dependencyKeys"].ToObject<List<string>>().ToArray());
                    bundles.Add(bundle.Key, bundle);
                }
            }
            
            Debug.LogError("SPTarkov.RuntimeBundles: Successfully received Bundles");
		}
    }

    public class BundleInfo
    {
        public string Key { get;}
        public string Path { get;}
        public string[] DependencyKeys { get;}

        public BundleInfo(string key, string path, string[] dependencyKeys)
        {
            Key = key;
            Path = path;
            DependencyKeys = dependencyKeys;
        }
    }
}
