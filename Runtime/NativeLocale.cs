using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Localization;

namespace NHNFramework.Localization
{
	public static class NativeLocale
	{
		public static Locale GetLocale(this List<Locale> availableLocales, string code, bool similar = false)
		{
			if (availableLocales != null && availableLocales.Count > 0)
			{
				foreach (var locale in availableLocales)
				{
					if (locale.Identifier.Code == code)
						return locale;
				}

				if (similar)
				{
					string subString = code.Substring(0, code.IndexOf("-"));
					foreach (var locale in availableLocales)
					{
						if (locale.Identifier.Code.Equals(subString, StringComparison.InvariantCultureIgnoreCase))
							return locale;
					}
					
				}
			}

			return null;
		}
		
		const string DefaultLanguageShort = "en";
		const string DefaultLanguageFull = "en-US";
		const string DefaultCountryCode = "en";

#if !UNITY_EDITOR && UNITY_IOS
	[DllImport("__Internal")]
	static extern string _CNativeLocaleGetLanguage();

	[DllImport("__Internal")]
	static extern string _CNativeLocaleGetCountryCode();
#endif

		static string DefaultLanguage(bool returnFull)
		{
			if (returnFull)
				return DefaultLanguageFull;
			return DefaultLanguageShort;
		}

		public static string GetLanguage()
		{
			return GetLanguage(false);
		}

		public static string GetLanguage(bool returnFullLanguage = true, bool returnNull = false)
		{
#if UNITY_EDITOR
			return returnNull ? null : DefaultLanguage(returnFullLanguage);
#elif UNITY_ANDROID
			try 
            { 
	            using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
	            {
		            using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
		            {
			            string lang = locale.Call<string>("getLanguage");

			            if (string.IsNullOrEmpty(lang))
				            return returnNull ? null : DefaultLanguage(returnFullLanguage);

			            if (!returnFullLanguage && lang.Length > 2)
				            lang = lang.Substring(0, 2);

			            return lang.ToLower();
		            }
	            }
            }
            catch (System.Exception)
            {
	            return returnNull ? null : DefaultLanguage(returnFullLanguage);
            }

		
#elif UNITY_IOS
		string lang = _CNativeLocaleGetLanguage();

		if (string.IsNullOrEmpty(lang))
			return returnNull ? null : DefaultLanguage(returnFullLanguage);

		if (!returnFullLanguage && lang.Length > 2)
			lang = lang.Substring(0, 2);

		return lang.ToLower();
#else
		 return returnNull ? null : DefaultLanguage(returnFullLanguage);
#endif
		}

		public static string GetCountryCode()
		{
#if UNITY_EDITOR
			return DefaultCountryCode;
#elif UNITY_ANDROID
		using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
		{
			using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
			{
				string cc = locale.Call<string>("getCountry");

				if (string.IsNullOrEmpty(cc))
					return DefaultCountryCode;

				if (cc.Length > 2)
					cc = cc.Substring(0, 2);

				return cc.ToLower();
			}
		}
#elif UNITY_IOS
		string cc = _CNativeLocaleGetCountryCode();

		if (string.IsNullOrEmpty(cc))
			return DefaultCountryCode;

		if (cc.Length > 2)
			cc = cc.Substring(0, 2);

		return cc.ToLower();
#else
		return DefaultCountryCode;
#endif
		}
	}
}