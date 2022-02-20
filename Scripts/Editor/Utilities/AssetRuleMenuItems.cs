using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetRules
{
    public static class AssetRuleMenuItems
    {
        public const string MENU_PATH = "Night Tools/Asset Rules";
        public const string ASSETS_MENU_PATH = "Assets";

        [MenuItem(ASSETS_MENU_PATH +"/Deprecate Asset", false, 19)]
        private static void DeprecateAsset()
        {
            foreach (Object asset in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                AssetDatabase.RenameAsset(assetPath, $"{Path.GetFileNameWithoutExtension(assetPath)}_Deprecated{DateTime.Now.ToString("ddMMM")}");
            }
        }
        
        [MenuItem("Assets/Check for Crimes", false,18)]
        private static void SearchForCrimes()
        {
            List<RuleBase.RuleResult> results = new List<RuleBase.RuleResult>();
            List<WarningRule.WarningResult> warnings = new List<WarningRule.WarningResult>();

            foreach (Object folder in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(folder);
                AssetRuleUtility.SearchForCrimesInFolder(assetPath, results,warnings);
            }
        }
        
        
        [MenuItem(AssetRuleMenuItems.ASSETS_MENU_PATH +"/Delete Empty Folders", false)]
        private static void DeleteEmptyFolders()
        {
            foreach (Object asset in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                RemoveEmptyFolders.CheckFolders(assetPath);
            }
        }
    }
}