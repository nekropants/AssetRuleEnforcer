using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.MENU_PATH +"/Rule For Unity Type" )]
    public class RuleForUnityType : RuleBase
    {
        public PrefabAssetType prefabType;
        
        public override bool DoesRuleApply(Object asset)
        {
            if (asset is GameObject)
            {
                PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(asset);
                return prefabAssetType == prefabType;
            }

            return false;
        }
    }
}