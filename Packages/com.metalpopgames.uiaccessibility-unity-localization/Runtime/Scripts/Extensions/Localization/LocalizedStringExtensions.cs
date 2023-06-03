using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace UAP.Extensions.Localization
{
    public static class LocalizedStringExtensions
    {
        private const string TAG = nameof(LocalizedString);
        
        public static string ResolveEntryKey(this LocalizedString localizedString)
        {
            var keyName = localizedString.TableEntryReference.ReferenceType switch
            {
                TableEntryReference.Type.Name => localizedString.TableEntryReference.Key,
                TableEntryReference.Type.Id => localizedString.GetEntryKeyName(),
                _ => string.Empty
            };

            return keyName;
        }

         
        // TODO: [Missing] Make this work with WebGL builds too with async requests (e.g use "StringDatabase.GetTableAsync()")
        // TODO: For now, fetch key name from localization tables is Unity Editor inspector only.
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static string GetEntryKeyName(this LocalizedString localizedString)
        {
            StringTable stringTable;

            #if UNITY_EDITOR

            if (LocalizationSettings.SelectedLocale == null)
            {
                var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection(localizedString.TableReference);
                var localeIdentifier = LocalizationSettings.ProjectLocale != null ? LocalizationSettings.ProjectLocale.Identifier : "en";

                stringTable = (StringTable)collection.GetTable(localeIdentifier);
            }
            else
            {
                stringTable = LocalizationSettings.StringDatabase?.GetTable(localizedString.TableReference);
            }
            #else
            
                stringTable = LocalizationSettings.StringDatabase?.GetTable(localizedString.TableReference);

            #endif

            return stringTable != null ? stringTable.GetEntry(localizedString.TableEntryReference.KeyId)?.Key : string.Empty;
        }
        
        public static bool IsMissingTranslation(
            this LocalizedString _,
            string value, 
            MissingTranslationBehavior missingTranslationState = MissingTranslationBehavior.PrintWarning
        )
        {
            var notFoundMessage = LocalizationSettings.StringDatabase.NoTranslationFoundMessage;
            var messagePrefix = !string.IsNullOrWhiteSpace(notFoundMessage) ? notFoundMessage[..14] : string.Empty;
            
            switch (LocalizationSettings.StringDatabase.MissingTranslationState)
            {
                case MissingTranslationBehavior.ShowMissingTranslationMessage when string.IsNullOrEmpty(value) || value.StartsWith(messagePrefix):
                {
                    if (missingTranslationState == MissingTranslationBehavior.PrintWarning && Debug.isDebugBuild)
                    {
                        Debug.LogWarning($"[{TAG}] {value}");
                    }

                    return true;
                }
                case MissingTranslationBehavior.PrintWarning:
                default:
                    return string.IsNullOrEmpty(value);
            }
        }
    }
}
