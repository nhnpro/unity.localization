using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Localization
{
    [CreateAssetMenu(menuName = "Localization/Locale Selectors/Saved Locale Selector")]
    public class SavedLocaleSelector : StartupLocaleSelector
    {
        public string Player_Pref_Key = "LANG_SAVED";
        public bool UseCustomID;
        public string CustomID = "";
        
        public override Locale GetStartupLocale(LocalesProvider availableLocales)
        {
            if (string.IsNullOrEmpty(Player_Pref_Key))
                return null;
            
            var savedKey = "";
            if (UseCustomID && string.IsNullOrEmpty(CustomID))
            {
                savedKey = PlayerPrefs.GetString(Player_Pref_Key + "_" + CustomID, "");
            }

            if (string.IsNullOrEmpty(savedKey))
            {
                savedKey = PlayerPrefs.GetString(Player_Pref_Key, "");
            }
            
            if (string.IsNullOrEmpty(savedKey))
                return null;

            var locale = availableLocales.GetLocale(savedKey);
            Debug.Log($"Detect Lang by SavedLocaleSelector :{savedKey} matched: {locale}");
            return locale;
        }
      
        
        void OnEnable()
        {
            if(Application.isPlaying)
                LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        void OnDisable()
        {
            if (Application.isPlaying)
                LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        void OnSelectedLocaleChanged(Locale selectedLocale)
        {
            if (UseCustomID && string.IsNullOrEmpty(CustomID))
            {
                PlayerPrefs.SetString(Player_Pref_Key + "_" + CustomID, selectedLocale.Identifier.Code);
            }
            else
            {
                PlayerPrefs.SetString(Player_Pref_Key, selectedLocale.Identifier.Code);
            }
        }
    }
}