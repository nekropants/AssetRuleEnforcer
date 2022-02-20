using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.MENU_PATH +"/Rule For Folder" )]
    public class RuleForFolder : RuleBase
    {
        public override bool DoesRuleApply(Object asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return Directory.Exists(path);
        }

        public override RuleResult GetResult(string assetPath)
        {
            RuleResult ruleResult = base.GetResult(assetPath);

            bool isEmpty = RemoveEmptyFolders.IsEmpty(assetPath);
            if (isEmpty)
            {
                ruleResult.requiresFix = true;
                ruleResult.customMessage = "Remove Empty Folder";
                ruleResult.fixAction = () =>                         AssetDatabase.DeleteAsset(ruleResult.oldPath);
            }
            
            return ruleResult;
        }
    }
}