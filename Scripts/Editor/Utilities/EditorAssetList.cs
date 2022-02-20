using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetRules
{
    /// <summary>
    /// Gets a list of asset in the project using AssetDatabase.FindAssets($"t:typeT") and caches the result 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct EditorAssetList<T> where T : Object
    {
        private static List<T> _list;

        private static List<T> list
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<T>();

                    string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

                    for (var i = 0; i < guids.Length; i++)
                    {
                        T ruleForType = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
                        _list.Add(ruleForType);
                    }
                }

                return _list;
            }
        }
        
        public int Count => list.Count;

        public T this[int i]
        {
            get => list[i];
            set => list[i] = value;
        }

        public IEnumerator GetEnumerator()
        {
            for (var index = 0; index < list.Count; index++)
            {
                yield return list[index];
            }
        }
    }
}