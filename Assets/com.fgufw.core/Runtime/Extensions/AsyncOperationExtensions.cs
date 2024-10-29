using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace FGUFW
{
    public static class AsyncOperationExtensions
    {
        static async public Task AsTask(this AsyncOperation self)
        {
            while (!self.isDone)
            {
                await Task.Yield();
            }
        }

    }
}