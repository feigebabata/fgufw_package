#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace FGUFW
{
    public class CreateScriptHelper : EndNameEditAction
    {
        public Func<string, string> Callback;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var scriptText = Callback(pathName);
            File.WriteAllText(pathName,scriptText,Encoding.UTF8);
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            var obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
            ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
        }
    }
}

#endif