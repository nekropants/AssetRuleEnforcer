using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AssetRules
{
    public abstract class RuleBase : ScriptableObject
    {
        static CultureInfo info = new CultureInfo("en-US", false);

        [Serializable]
        public struct FindReplace
        {
            public bool disable;
            public string replace;
            public string with;
        }

        public class RuleResult
        {
            public string oldPath;
            public string newPath;
            public string commonPath;
            public RuleBase rule;
            public bool requiresFix;
            public string customMessage;
            public Action fixAction;
        }

        [SerializeField] public Object _example;

        [SerializeField] protected string _customPrefix;
        [SerializeField] protected string _customPostfix;
        [SerializeField] protected string _regexValidation;
        [SerializeField] protected bool _removeSpaces = false;
        [SerializeField] protected bool _replaceSpacesWithUnderscores = true;

  [SerializeField]
        protected FindReplace[] _regexReplaceFromFromName;

        
        [SerializeField]
        protected FindReplace[] _regexReplaceInDirectories;
        [SerializeField] protected string _category;

        [Header("Tests")] 

        [SerializeField] public Object _test;
        [SerializeField] public string _pathTest;

        public Type type => _example.GetType();
        public string typeName => _example.GetType().Name;
  
        private void OnValidate()
        {
            _category = _category.Trim();
        }

        public bool IsRegexValid(string assetName)
        {
            return Regex.IsMatch(assetName, _regexValidation);
        }

        public static string RegexReplace(string assetName, FindReplace[] list)
        {
            if (list == null)
                return assetName;

            string name = assetName;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].disable == false)
                    name = Regex.Replace(name, list[i].replace, list[i].with);
            }

            return name;
        }

        public static string RegexRemove(string assetName, string[] list)
        {
            if (list == null)
                return assetName;

            string name = assetName;
            for (int i = 0; i < list.Length; i++)
            {
                name = Regex.Replace(name, list[i], "");
            }

            return name;
        }

        public string GetExample()
        {
            return Format("AssetPath/Example");
        }

        public string ApplyCategory(string formattedName)
        {
            if (_category != "")
            {
                formattedName = $"{_category}/{formattedName}";
            }

            return formattedName;
        }

        public virtual string Format(string assetPath)
        {
            // name
            string formattedName = Path.GetFileNameWithoutExtension(assetPath);

            formattedName = RegexReplace(formattedName, _regexReplaceFromFromName);

            if (_customPrefix != "")
            {
                formattedName = Regex.Replace(formattedName, _customPrefix.Replace("_", ""), "");

                if (Regex.IsMatch(formattedName, $"^({_customPrefix})") == false) // check if prefix already exists
                {
                    formattedName = $"{_customPrefix}{formattedName}";
                }
            }

            if (_customPostfix != "" && Regex.IsMatch(formattedName, $"({_customPostfix}$)") == false)
                formattedName = $"{formattedName}{_customPostfix}";


            formattedName = formattedName.Replace("__", "_");

            // formattedName = info.TextInfo.ToTitleCase(formattedName);

            if (_removeSpaces)
                formattedName = formattedName.Replace(" ", "");

            if (_replaceSpacesWithUnderscores)
                formattedName = formattedName.Replace(" ", "_");

            formattedName = formattedName.Replace("__", "_");


            // directory
            string formattedDirectory = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "/";

            formattedDirectory = ToTitleCase(formattedDirectory);
            formattedDirectory = RegexReplace(formattedDirectory, _regexReplaceInDirectories);


            if (_category != "")
            {
                string parentName;
                DirectoryInfo parent = Directory.GetParent(assetPath);
                parentName = parent.Name;

                if (parentName != "Resources" && Regex.IsMatch(formattedDirectory, $"/{_category}/") == false)
                {
                    formattedDirectory = $"{formattedDirectory}{_category}/";
                }
            }

            return $"{formattedDirectory}{formattedName}{Path.GetExtension(assetPath)}";
        }

            public static string ToTitleCase( string str)
            {
                var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 0; i < tokens.Length; i++)
                {
                    var token = tokens[i];
                    tokens[i] = token == token.ToUpper()
                        ? token 
                        : token.Substring(0, 1).ToUpper() + token.Substring(1);
                }

                return string.Join(" ", tokens);
            }
        
        public virtual bool DoesRuleApply(Object asset)
        {
            return false;
        }

        public virtual RuleResult GetResult(string assetPath)
        {
            RuleResult result;

            assetPath = assetPath.Replace("\\", "/");
            string newPath = Format(assetPath);
            
            result = new RuleBase.RuleResult()
            {
                oldPath = assetPath,
                newPath = newPath,
                rule = this,
                // commonPath = " "
                commonPath = AssetRuleUtility.GetCommon(assetPath, newPath )
            };
            if (assetPath != newPath)
            {
                result.requiresFix = true;
                result.fixAction = () =>       AssetRuleUtility.MoveAndRename(result.oldPath, result.newPath);
            }

            return result;
        }
    }
}