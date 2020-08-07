using IBundleLock = GInterface224; //Property: IsLocked
using BindableState = GClass2037<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue
using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace SPTarkov.RuntimeBundles.Utils
{
    class EasyBundleHelper
    {
        private readonly object _instance;
        private readonly Traverse _trav;

        private static string pathFieldName = "string_1";
        private static string _keyWithoutExtensionFieldName = "string_0";
        private static string _bundleLockPropertyName = "ginterface224_0";


        public IEnumerable<string> DependencyKeys
        {
            get
            {
                return _trav.Property<IEnumerable<string>>("DependencyKeys").Value;
            }

            set
            {
                _trav.Property<IEnumerable<string>>("DependencyKeys").Value = value;
            }
        }
        public IBundleLock BundleLock
        {
            get
            {
                return _trav.Field<IBundleLock>(_bundleLockPropertyName).Value;
            }

            set
            {
                _trav.Field<IBundleLock>(_bundleLockPropertyName).Value = value;
            }
        }

        public Task LoadingJob
        {
            get
            {
                return _trav.Field<Task>("task_0").Value;
            }

            set
            {
                _trav.Field<Task>("task_0").Value = value;
            }
        }

        public string Path
        {
            get
            {
                return _trav.Field<string>("string_1").Value;
            }

            set
            {
                _trav.Field<string>("string_1").Value = value;
            }
        }

        public string Key
        {
            get
            {
                return _trav.Property<string>("Key").Value;
            }

            set
            {
                _trav.Property<string>("Key").Value = value;
            }
        }

        public BindableState LoadState
        {
            get
            {
                return _trav.Property<BindableState>("LoadState").Value;
            }

            set
            {
                _trav.Property<BindableState>("LoadState").Value = value;
            }
        }
        public float Progress
        {
            get
            {
                return _trav.Property<float>("Progress").Value;
            }

            set
            {
                _trav.Property<float>("Progress").Value = value;
            }
        }


        public AssetBundle Bundle
        {
            get
            {
                return _trav.Field<AssetBundle>("assetBundle_0").Value;
            }

            set
            {
                _trav.Field<AssetBundle>("assetBundle_0").Value = value;
            }
        }

        public AssetBundleRequest loadingAssetOperation
        {
            get
            {
                return _trav.Field<AssetBundleRequest>("assetBundleRequest_0").Value;
            }

            set
            {
                _trav.Field<AssetBundleRequest>("assetBundleRequest_0").Value = value;
            }
        }


        public Object[] Assets
        {
            get
            {
                return _trav.Property<UnityEngine.Object[]>("Assets").Value;
            }

            set
            {
                _trav.Property<UnityEngine.Object[]>("Assets").Value = value;
            }
        }

        public UnityEngine.Object SameNameAsset
        {
            get
            {
                return _trav.Property<UnityEngine.Object>("SameNameAsset").Value;
            }

            set
            {
                _trav.Property<UnityEngine.Object>("SameNameAsset").Value = value;
            }
        }

        public string KeyWithoutExtension
        {
            get
            {
                return _trav.Field<string>("string_0").Value;
            }

            set
            {
                _trav.Field<string>("string_0").Value = value;
            }
        }


        public EasyBundleHelper(object easyBundle)
        {
            this._instance = easyBundle;
            _trav = Traverse.Create(easyBundle);
        }

        public Task LoadingCoroutine()
        {
            return _trav.Method("method_0").GetValue<Task>();
        }
    }
}
