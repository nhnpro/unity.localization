using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization
{
    /// <summary>
    /// Performs all initialization work for the LocalizationSettings.
    /// </summary>
    public class InitializationOperation : AsyncOperationBase<LocalizationSettings>
    {
        private int m_PreloadingOperations;
        private LocalizationSettings m_Settings;

        /// <inheritdoc />
        public override void ResetStatus()
        {
            // Remove ourself from any preloading operations or we may have duplicate callbacks.
            if (m_PreloadingOperations > 0)
            {
                if (m_Settings.GetAssetDatabase() is IPreloadRequired assetOperation)
                    assetOperation.PreloadOperation.Completed -= PreloadOperationCompleted;

                if (m_Settings.GetStringDatabase() is IPreloadRequired stringOperation)
                    stringOperation.PreloadOperation.Completed -= PreloadOperationCompleted;
                
            }

            base.ResetStatus();
            m_PreloadingOperations = 0;
            m_Settings = null;
        }

        public virtual InitializationOperation Start(LocalizationSettings settings)
        {
            Debug.Log("Go here 1");
            m_Settings = settings;

            // First time initialization requires loading locales and selecting the startup locale without sending a locale changed event.
            if (m_Settings.GetSelectedLocale() == null)
            {
                Debug.Log("Go here 2");
                // Load Locales
                if (m_Settings.GetAvailableLocales() is IPreloadRequired locales && !locales.PreloadOperation.IsDone)
                {
                    Debug.Log("Go here 3");
                    locales.PreloadOperation.Completed += (async) =>
                    {
                        Debug.Log("Go here 4");
                        m_Settings.InitializeSelectedLocale();
                        PreLoadTables();
                    };
                    Debug.Log("Go here 5");
                    return this;
                }
                Debug.Log("Go here 6");
                m_Settings.InitializeSelectedLocale();
            }

            Debug.Log("Go here 7");
            PreLoadTables();
            return this;
        }

        private void PreloadOperationCompleted(IAsyncOperation obj)
        {
            m_PreloadingOperations--;

            if (obj.HasLoadedSuccessfully())
            {
                Status = obj.Status;
                m_Error = obj.OperationException;
            }

            Debug.Assert(m_PreloadingOperations >= 0);
            if (m_PreloadingOperations == 0)
                FinishInitializing();
        }

        void PreLoadTables()
        {
            if (m_Settings.PreloadBehavior == PreloadBehavior.OnDemand)
            {
                FinishInitializing();
                return;
            }

            Debug.Assert(m_PreloadingOperations == 0);
            m_PreloadingOperations = 0;
            if (m_Settings.GetAssetDatabase() is IPreloadRequired assetOperation)
            {
                Debug.Log("Localization: Preloading Asset Tables(" + Time.timeSinceLevelLoad + ")");
                if (!assetOperation.PreloadOperation.IsDone)
                {
                    assetOperation.PreloadOperation.Completed += (async) =>
                    {
                        Debug.Log("Localization: Finished Preloading Asset Tables(" + Time.timeSinceLevelLoad + ")");
                        PreloadOperationCompleted(async);
                    };
                    m_PreloadingOperations++;
                }
            }

            if (m_Settings.GetStringDatabase() is IPreloadRequired stringOperation)
            {
                Debug.Log("Localization: Preloading String Tables(" + Time.timeSinceLevelLoad + ")");
                if (!stringOperation.PreloadOperation.IsDone)
                {
                    stringOperation.PreloadOperation.Completed += (async) =>
                    {
                        Debug.Log("Localization: Finished Preloading String Tables(" + Time.timeSinceLevelLoad + ")");
                        PreloadOperationCompleted(async);
                    };
                    m_PreloadingOperations++;
                }
            }

            if (m_PreloadingOperations == 0)
                FinishInitializing();
        }

        void FinishInitializing()
        {
            Debug.Log("Go here 8 " + Status);
            if (Status != AsyncOperationStatus.Failed)
                Status = AsyncOperationStatus.Succeeded;
            InvokeCompletionEvent();
        }
    }
}