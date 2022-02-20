using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    [CustomEditor(typeof(RuleForUnityType))]
    public class NamingRuleForUnityTypeEditor : AssetNamingRuleEditor<RuleForUnityType>
    {
    }

    [CustomEditor(typeof(RuleForTexture))]
    public class NamingRuleForTextureEditor : AssetNamingRuleEditor<RuleForTexture>
    {
    }
    
    [CustomEditor(typeof(RuleForSystemType))]
    public class AssetNamingRuleEditor : AssetNamingRuleEditor<RuleForSystemType>
    {
    }    
    
    [CustomEditor(typeof(RuleForFolder))]
    public class RuleForFolderEditor : AssetNamingRuleEditor<RuleForFolder>
    {
    }
    
    public class AssetNamingRuleEditor<T> : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RuleBase ruleBase = target as RuleBase;
            GUILayout.Space(30);

            if (ruleBase._example == null)
            {
                GUILayout.Label("No Example Type Assigned");
            }
            else
            {
                GUIStyle heading = new GUIStyle(GUI.skin.label);
                heading.fontStyle = FontStyle.Bold;
                GUILayout.Label($"Preview Output:", heading);

                GUILayout.Label($"Type: {ruleBase.type}");
                string example = ruleBase.GetExample();
                GUILayout.Label($"Example: {example}");

                string withCategory = ruleBase.ApplyCategory(example);
                if (example != withCategory)
                    GUILayout.Label("Result: " + withCategory);

                if (ruleBase._test)
                {
                    GUILayout.Label("");
                    GUILayout.Label($"Asset Test Output:", heading);
                    string test = ruleBase.Format($"Assets/{ruleBase._test.name}");
                    GUILayout.Label($"Test:   {ruleBase._test.name}");
                    GUILayout.Label($"Test Type:   {ruleBase._test.GetType()}");
                    GUILayout.Label($"Rule Applies:   {ruleBase.DoesRuleApply(ruleBase._test)}");
                    GUILayout.Label($"Result: {test}");
                }

                if (ruleBase._pathTest != "")
                {
                    GUILayout.Label($"");
                    GUILayout.Label($"Path Test Output:", heading);
                    RuleBase.RuleResult ruleResult = ruleBase.GetResult(ruleBase._pathTest);
                    GUILayout.Label(ruleBase._pathTest);
                    GUILayout.Label(ruleResult.newPath);
                    GUILayout.Label(ruleResult.customMessage);
                }
            }

            GUILayout.Label("");

            if (GUILayout.Button("Search for Crimes"))
            {
                AssetRuleUtility.SearchForCrimesByRule(ruleBase);
            }
        }
    }
}