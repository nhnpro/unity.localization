using System.Globalization;
using NHNFramework.Localization;
using UnityEngine;

namespace UnityEngine.Localization
{
    [CreateAssetMenu(menuName = "Localization/Locale Selectors/Device Locale Selector")]
    public class DeviceLocaleSelector : StartupLocaleSelector
    {
       //Example en-US => en
        public bool AllowSimillarCoutryCode = true;
        public override Locale GetStartupLocale(LocalesProvider availableLocales)
        {
            Locale locale = null;
            
            string currentDeviceLanguage = NativeLocale.GetLanguage(true, true);
            if (!string.IsNullOrEmpty(currentDeviceLanguage))
            {
                locale = availableLocales.Locales.GetLocale(currentDeviceLanguage, AllowSimillarCoutryCode);
            }
            Debug.Log($"Detect Lang By Device:{currentDeviceLanguage} matched locale {locale}");
            
            if (locale != null) return locale;
            
            if (Application.systemLanguage != SystemLanguage.Unknown)
            {
                locale = availableLocales.GetLocale(Application.systemLanguage);
            }

            if (locale == null)
            {
                var cultureInfo = CultureInfo.CurrentUICulture;
                locale = availableLocales.GetLocale(cultureInfo);
                if (locale == null)
                {
                    // Attempt to use CultureInfo fallbacks to find the closest locale
                    while (!Equals(cultureInfo, CultureInfo.InvariantCulture) && locale == null)
                    {
                        locale = availableLocales.GetLocale(cultureInfo);
                        cultureInfo = cultureInfo.Parent;
                    }

                    if (locale != null)
                    {
                        Debug.Log(
                            $"Locale '{CultureInfo.CurrentUICulture}' is not supported, however the parent locale '{locale.Identifier.CultureInfo}' is.");
                    }
                }
            }
            return locale;
        }
    }
}