using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    [CreateAssetMenu(menuName = AssetRuleMenuItems.MENU_PATH )]
    public class RuleForTexture: RuleBase
    {
        public TextureImporterType textureType;
        public string contains;
        public override bool DoesRuleApply(Object asset)
        {
            if (asset is Texture2D)
            {
                string path = AssetDatabase.GetAssetPath(asset);

                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

                
                if (importer.textureType != textureType)
                    return false;


                if (string.IsNullOrWhiteSpace(contains) == false)
                {
                    if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path), $"{contains}", RegexOptions.IgnoreCase) == false)
                    {
                        return false;
                    }
                }
                
                return true;

            }

            return false;
        }
    }
}