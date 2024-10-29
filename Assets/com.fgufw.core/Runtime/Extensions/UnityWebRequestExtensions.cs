using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FGUFW
{
    public static class UnityWebRequestExtensions
    {
        public static IEnumerator StreamingCopy(string originPath,string savePath,Action<string> complete)
        {
            var url = new Uri(Path.Combine(Application.streamingAssetsPath,originPath));
            UnityWebRequest uwr = new UnityWebRequest(url);
            uwr.downloadHandler = new DownloadHandlerFile(savePath);
            yield return uwr.SendWebRequest();
            complete?.Invoke(uwr.error);
        }

        public static async Task<UnityWebRequestAsyncOperation> RequestAsync( this UnityWebRequest self)
        {
            var request = self.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Delay(40);// 25帧
            }
            return request;
        }

    }
}