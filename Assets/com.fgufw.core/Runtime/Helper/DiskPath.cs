using System.IO;
using System;
using System.Collections.Generic;

namespace FGUFW
{
    /// <summary>
    /// 磁盘路径助手
    /// </summary>
    public sealed class DiskPath
    {
        public List<string> Directorys{get;private set;} = new List<string>();
        public List<string> Files{get;private set;} = new List<string>();
        public string Path{get;private set;}

        private DiskPath(){}

        public DiskPath(string path)
        {
            SetPath(path);
        }

        public void Back()
        {
            var newPath = System.IO.Path.GetDirectoryName(Path);
            SetPath(newPath);
        }


        public void SetPath(string path)
        {
            Directorys.Clear();
            Files.Clear();

            if(Directory.Exists(path))
            {
                foreach (var item in Directory.GetDirectories(path))
                {
                    Directorys.Add(item);
                }
                foreach (var item in Directory.GetFiles(path))
                {
                    Files.Add(item);
                }
                Path = path;
            }
            else
            {
#if UNITY_EDITOR
                Path = null;
                foreach (var item in Directory.GetLogicalDrives())
                {
                    Directorys.Add(item);
                }
#elif UNITY_ANDROID
			    Path = UnityEngine.Application.persistentDataPath.Replace($"Android/data/{UnityEngine.Application.identifier}/files","");
                foreach (var item in Directory.GetDirectories(Path))
                {
                    Directorys.Add(item);
                }
                foreach (var item in Directory.GetFiles(Path))
                {
                    Files.Add(item);
                }
#endif
            }
        }

    }
}