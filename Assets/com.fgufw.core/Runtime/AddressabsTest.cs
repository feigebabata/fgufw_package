using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FGUFW
{
    public class AddressabsTest
    {
        public static GameObject Copy(string path,Transform parent)
        {
            return Addressables.InstantiateAsync(path,parent).WaitForCompletion();
        }
    }

}