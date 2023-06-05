using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UAP.Extensions.Localization;

namespace UAP.ScriptableObjects.LocalizationStrategies
{
    [CreateAssetMenu(fileName = "UnityLocalization", menuName = "UAP/Localization/UnityLocalization")]
    public class UnityLocalizationStrategy : LocalizationStrategy
    {
        private const string TAG = nameof(LocalizationStrategy);
        
        public virtual LocalizeStringEvent Initialize(ref string key, MonoBehaviour elementWithLocalize = null)
        {
            
            if (elementWithLocalize is null)
            {
                key = string.Empty;
                return null;
            }
            
            if (!elementWithLocalize.TryGetComponent<LocalizeStringEvent>(out var localizeStringEvent))
            {
                localizeStringEvent = elementWithLocalize.gameObject.AddComponent<LocalizeStringEvent>();
            }

            localizeStringEvent.StringReference ??= new LocalizedString();
            
            if (string.IsNullOrEmpty(localizeStringEvent.StringReference.TableReference))
            {
                localizeStringEvent.SetTable(LocalizationSettings.StringDatabase.DefaultTable.TableCollectionName);
            }
            
            if (!string.IsNullOrWhiteSpace(key))
            {
                localizeStringEvent.SetEntry(key);
            }
            else if (localizeStringEvent.StringReference.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty)
            {
                var tableKey = localizeStringEvent.StringReference.ResolveEntryKey();

                if (tableKey != null && tableKey != key)
                {
                    key = tableKey;
                }
            }
            else if (Debug.isDebugBuild)
            {
                Debug.LogWarning($"[{TAG}] localization KEY is empty!");
                return null;
            }
            
            if (LocalizationSettings.SelectedLocale == null)
            {
                try
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.ProjectLocale;
                }
                catch (NotSupportedException ex)
                {
                    if (Debug.isDebugBuild)
                    {
                        // Try avoid NotSupportedException error for "LocalizeStringEvent" component
                        Debug.LogWarning($"[{ex.GetType().Name}]: {ex.Message}");
                    }
                }
            }

            return localizeStringEvent;
        }

        public override string Localize(ref string key, MonoBehaviour elementWithLocalize = null)
        {
            // Initialize "LocalizeStringEvent" component settings
            var localizeStringEvent = Initialize(ref key, elementWithLocalize);

            if (localizeStringEvent == null)
            {
                return string.Empty;
            }
            
            var localizedString = localizeStringEvent.StringReference;
            var localizedValue = localizedString.GetLocalizedString();
            
            return localizedString.IsMissingTranslation(localizedValue) ? key : localizedValue;
        }

        [SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
        public override Task<string> LocalizeAsync(ref string key, MonoBehaviour elementWithLocalize = null)
        {
            // Initialize "LocalizeStringEvent" component settings
            var localizeStringEvent = Initialize(ref key, elementWithLocalize);
            var keyToTranslate = key;

            if (localizeStringEvent == null)
            {
                return Task.FromResult(string.Empty);
            }

            var localizedString = localizeStringEvent.StringReference;

            #if !UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS
            
            // Fallback to synchronously translation (except on WEBGL builds)
            if (LocalizationSettings.InitializeSynchronously || localizedString.WaitForCompletion)
            {
                var localizedValue = localizedString.GetLocalizedString();
                if (localizedString.IsMissingTranslation(localizedValue))
                {
                    localizedValue = key;
                }
                
                return Task.FromResult(localizedValue);
            }
            
            #endif
            
            // Get the translate string async
            var asyncOperation = localizedString.GetLocalizedStringAsync();
            
            return asyncOperation.Task.ContinueWith(localizedValueTask =>
            {
                return localizedString.IsMissingTranslation(localizedValueTask.Result) ? keyToTranslate : localizedValueTask.Result;
            });
        }
    }
}
