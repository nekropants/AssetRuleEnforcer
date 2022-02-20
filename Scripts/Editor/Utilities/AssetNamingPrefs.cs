using System.Collections.Generic;
using UnityEditor;

namespace AssetRules
{
    [FilePath("NightTools/AssetNamingPrefs.prefs", FilePathAttribute.Location.PreferencesFolder)]
    public class AssetNamingPrefs : ScriptableSingleton<AssetNamingPrefs>
    {
        public List<string> _ignoreList = new List<string>();
    }
}