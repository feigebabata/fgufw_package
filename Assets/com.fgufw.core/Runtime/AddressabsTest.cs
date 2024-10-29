using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace FGUFW
{
    public class AddressabsTest
    {
        public static GameObject Copy(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent).WaitForCompletion();
        }

        public static T Load<T>(string path)
        {
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }

        public static SceneInstance LoadScene(string path)
        {
            return Addressables.LoadSceneAsync(path).WaitForCompletion();
        }
        
    }

}