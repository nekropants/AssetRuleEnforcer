using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AssetRules
{
    class AssetPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            
            List<RuleBase.RuleResult> results = new List<RuleBase.RuleResult>();
            List<WarningRule.WarningResult> warnings = new List<WarningRule.WarningResult>() ;

            for (var i = 0; i < importedAssets.Length; i++)
            {
                // Debug.Log("post " + importedAssets.Length);

                string str = importedAssets[i];
                AssetRuleUtility.CheckRule(str,results, warnings);
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                AssetRuleUtility.CheckRule(movedAssets[i], results,warnings);
            }
            
            if(results.Count > 0)
                AssetRuleUtility.DisplayPopup(results,warnings);
        }
    }
}