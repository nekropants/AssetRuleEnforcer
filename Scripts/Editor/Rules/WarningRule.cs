using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.MENU_PATH +"/Warning" )]
    public class WarningRule : ScriptableObject
    {
        [SerializeField] private bool size = false;
        [SerializeField] private float sizeMB = 10;

        public class WarningResult
        {
            public Object asset;
            public string message;
        }


        public  bool DoesRuleApply(Object asset)
        {
            if (size)
            {

                string filePath = AssetDatabase.GetAssetPath(asset);
                FileAttributes fileAttributes = File.GetAttributes(filePath);
                long length = new System.IO.FileInfo(filePath).Length;
                double fileSizeInMbs = length / (1024.0 * 1024);

                return fileSizeInMbs > sizeMB;
            }

            return false;
        }

        public WarningResult CheckForWarning(Object asset)
        {
            
            WarningResult warningResult = null;
            if (size)
            {

                string filePath = AssetDatabase.GetAssetPath(asset);
                if (File.Exists(filePath))
                {

                    FileAttributes fileAttributes = File.GetAttributes(filePath);
                    long length = new System.IO.FileInfo(filePath).Length;
                    double fileSizeInMbs = length / (1024.0 * 1024);

                    if (fileSizeInMbs > sizeMB)
                        warningResult = new WarningResult()
                            { asset = asset, message = $"{Math.Round(fileSizeInMbs, 1)}mb - {filePath}" };
                }
            }

            return warningResult;
        }
        
    }
}