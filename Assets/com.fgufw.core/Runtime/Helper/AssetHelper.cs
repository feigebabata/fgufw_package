using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FGUFW
{
    public static class AssetHelper
    {

        public static Task<T> LoadAsync<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path).Task;
        }

        public static Task<IList<T>> LoadsAsync<T>(string path,Action<T> callback=null)
        {
            return Addressables.LoadAssetsAsync<T>(path,callback).Task;
        }

        public static T Load<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }

        public static IList<T> Loads<T>(string path)
        {
            return Addressables.LoadAssetsAsync<T>(path,null).WaitForCompletion();
        }

        public static Task<GameObject> CopyAsync(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent).Task;
        }

        public static GameObject Copy(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent).WaitForCompletion();
        }

        public static Task LoadSceneAsync(string path)
        {
            return Addressables.LoadSceneAsync(path).Task;
        }

        public static SceneInstance LoadScene(string path)
        {
            return Addressables.LoadSceneAsync(path).WaitForCompletion();
        }

        public static IEnumerator LoadScene(string path,LoadSceneMode loadSceneMode= LoadSceneMode.Single)
        {
            yield return Addressables.LoadSceneAsync(path,loadSceneMode);
        }

        public static AsyncOperationHandle<GameObject> CopyAsynchronous(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent);
        }
    }


}
