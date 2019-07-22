using System;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Components
{
    [AddComponentMenu("Localization/Generic/String")]
    public class LocalizeString : LocalizationBehaviour
    {
        [Serializable]
        public class LocalizationUnityEvent : UnityEvent<string> { };

        [SerializeField]
        bool m_IsPlural;

        [SerializeField]
        int m_PluralValue = 1;
        
        [field: SerializeField]
        public LocalizedStringReference StringReference { get; set; } = new LocalizedStringReference();

        [field: SerializeField]
        public LocalizationUnityEvent UpdateString { get; set; } = new LocalizationUnityEvent();

        public bool IsPlural
        {
            get => m_IsPlural;
            set
            {
                if(m_IsPlural == value)
                    return;

                m_IsPlural = value; 
                ForceUpdate();
            }
        }

        public int PluralValue
        {
            get => m_PluralValue;
            set
            {
                if (m_PluralValue == value)
                    return;

                m_PluralValue = value; 

                if(IsPlural)
                    ForceUpdate();
            }
        }

        protected override void OnLocaleChanged(Locale newLocale)
        {
            var stringOperation = m_IsPlural ? StringReference.GetLocalizedString(m_PluralValue) : StringReference.GetLocalizedString();
            stringOperation.Completed += StringLoaded;
        }

        protected virtual void StringLoaded(IAsyncOperation<string> stringOp)
        {
            if (stringOp.HasLoadedSuccessfully())
            {
                UpdateString.Invoke(stringOp.Result);
            }
        }
    }
}