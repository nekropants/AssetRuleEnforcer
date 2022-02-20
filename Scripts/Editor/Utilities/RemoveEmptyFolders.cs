using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    /// <summary>
    /// Remove empty folders automatically.
    /// </summary>
    public class RemoveEmptyFolders : UnityEditor.AssetModificationProcessor
    {
        public const string kMenuText = "Assets/Remove Empty Folders";
        static readonly StringBuilder s_Log = new StringBuilder();
        static readonly List<DirectoryInfo> s_Results = new List<DirectoryInfo>();

        
        /// <summary>
        /// Raises the will save assets event.
        /// </summary>
        public static void CheckFolders(string dir)
        {
            if (Directory.Exists(dir) == false)
            {
                return;
            }
            var assetsDir = Application.dataPath + Path.DirectorySeparatorChar;
            assetsDir = assetsDir.Replace("/", "\\");
            assetsDir = assetsDir.Replace("Assets\\", "");

            // Get empty directories in Assets directory
            s_Results.Clear();
            GetEmptyDirectories(new DirectoryInfo(dir), s_Results);

            // When empty directories has detected, remove the directory.
            if (0 < s_Results.Count)
            {
                s_Log.Length = 0;
                s_Log.AppendFormat("Remove {0} empty directories as following:\n", s_Results.Count);
                foreach (var d in s_Results)
                {
                    string relativePath = d.FullName.Replace(assetsDir, "");
                    s_Log.AppendFormat("- {0}\n", relativePath);
                    AssetDatabase.DeleteAsset(relativePath);
                }

                // UNITY BUG: Debug.Log can not set about more than 15000 characters.
                s_Log.Length = Mathf.Min(s_Log.Length, 15000);
                Debug.Log(s_Log.ToString());
                s_Log.Length = 0;

                AssetDatabase.Refresh();
            }
        }


        /// <summary>
        /// Get empty directories.
        /// </summary>
        static bool GetEmptyDirectories(DirectoryInfo dir, List<DirectoryInfo> results)
        {

            bool isEmpty = true;
            try
            {
                isEmpty = dir.GetDirectories().Count(x => !GetEmptyDirectories(x, results)) == 0	// Are sub directories empty?
                          && dir.GetFiles("*.*").All(x => x.Extension == ".meta");	// No file exist?
            }
            catch
            {
            }

            // Store empty directory to results.
            if (isEmpty)
                results.Add(dir);
            return isEmpty;
        }
        
        public static bool IsEmpty(string assetPath)
        {
            var assetsDir = Application.dataPath.Replace("/Assets","") + "/" + assetPath;

            DirectoryInfo dir = new DirectoryInfo(assetsDir);
            if (dir.Exists == false)
                return false;
            
            bool isEmpty = false;
            try
            {
                isEmpty = dir.GetFiles("*.*").All(x => x.Extension == ".meta") && dir.GetDirectories().Length == 0;	// No file exist?
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            return isEmpty;
        }
    }
}