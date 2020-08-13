using IBundleLock = GInterface224; //Property: IsLocked
using BindableState = GClass2080<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue
using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SPTarkov.RuntimeBundles.Utils
{
    class EasyBundleHelper
    {
        private readonly object _instance;
        private readonly Traverse _trav;

        private static readonly string _pathFieldName = "string_1";
        private static readonly string _keyWithoutExtensionFieldName = "string_0";
        private static readonly string _bundleLockPropertyName = "ginterface224_0";
        private static readonly string _loadingJobPropertyName = "task_0";
        private static readonly string _dependencyKeysPropertyName = "DependencyKeys";
        private static readonly string _keyPropertyName = "Key";
        private static readonly string _loadStatePropertyName = "LoadState";
        private static readonly string _progressPropertyName = "Progress";
        private static readonly string _bundlePropertyName = "Bundle";
        private static readonly string _loadingAssetOperationFieldName = "assetBundleRequest_0";
        private static readonly string _assetsPropertyName = "Assets";
        private static readonly string _sameNameAssetPropertyName = "SameNameAsset";
        private static MethodInfo _loadingCoroutineMethod;



        public IEnumerable<string> DependencyKeys
        {
            get
            {
                return _trav.Property<IEnumerable<string>>(_dependencyKeysPropertyName).Value;
            }

            set
            {
                _trav.Property<IEnumerable<string>>(_dependencyKeysPropertyName).Value = value;
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
                return _trav.Field<Task>(_loadingJobPropertyName).Value;
            }

            set
            {
                _trav.Field<Task>(_loadingJobPropertyName).Value = value;
            }
        }

        public string Path
        {
            get
            {
                return _trav.Field<string>(_pathFieldName).Value;
            }

            set
            {
                _trav.Field<string>(_pathFieldName).Value = value;
            }
        }

        public string Key
        {
            get
            {
                return _trav.Property<string>(_keyPropertyName).Value;
            }

            set
            {
                _trav.Property<string>(_keyPropertyName).Value = value;
            }
        }

        public BindableState LoadState
        {
            get
            {
                return _trav.Property<BindableState>(_loadStatePropertyName).Value;
            }

            set
            {
                _trav.Property<BindableState>(_loadStatePropertyName).Value = value;
            }
        }
        public float Progress
        {
            get
            {
                return _trav.Property<float>(_progressPropertyName).Value;
            }

            set
            {
                _trav.Property<float>(_progressPropertyName).Value = value;
            }
        }

        
        public AssetBundle Bundle
        {
            get
            {
                return _trav.Field<AssetBundle>(_bundlePropertyName).Value;
            }

            set
            {
                _trav.Field<AssetBundle>(_bundlePropertyName).Value = value;
            }
        }

        
        public AssetBundleRequest loadingAssetOperation
        {
            get
            {
                return _trav.Field<AssetBundleRequest>(_loadingAssetOperationFieldName).Value;
            }

            set
            {
                _trav.Field<AssetBundleRequest>(_loadingAssetOperationFieldName).Value = value;
            }
        }


        public Object[] Assets
        {
            get
            {
                return _trav.Property<UnityEngine.Object[]>(_assetsPropertyName).Value;
            }

            set
            {
                _trav.Property<UnityEngine.Object[]>(_assetsPropertyName).Value = value;
            }
        }

        public UnityEngine.Object SameNameAsset
        {
            get
            {
                return _trav.Property<UnityEngine.Object>(_sameNameAssetPropertyName).Value;
            }

            set
            {
                _trav.Property<UnityEngine.Object>(_sameNameAssetPropertyName).Value = value;
            }
        }

        public string KeyWithoutExtension
        {
            get
            {
                return _trav.Field<string>(_keyWithoutExtensionFieldName).Value;
            }

            set
            {
                _trav.Field<string>(_keyWithoutExtensionFieldName).Value = value;
            }
        }


        public EasyBundleHelper(object easyBundle)
        {
            this._instance = easyBundle;
            _trav = Traverse.Create(easyBundle);

            if (_loadingCoroutineMethod == null)
            {
                _loadingCoroutineMethod = easyBundle.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(x => x.GetParameters().Length == 0 && x.ReturnType == typeof(Task));
                //TODO:Search member names by condition
            }
        }

        public Task LoadingCoroutine()
        {
            return (Task)_loadingCoroutineMethod.Invoke(_instance, new object[] { });
        }
    }
}
