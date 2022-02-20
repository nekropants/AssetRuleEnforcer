using UnityEngine;

namespace AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.MENU_PATH + "Rule for System Type ")]
    public class RuleForSystemType : RuleBase
    {
        public override bool DoesRuleApply(Object asset)
        {
            return asset.GetType() == type;
        }
    }
}