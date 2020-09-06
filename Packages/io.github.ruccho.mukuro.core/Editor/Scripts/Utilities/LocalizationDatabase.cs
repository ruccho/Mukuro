using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Mukuro.Editors.Utilities
{
    public static class LocalizationDatabase
    {
        private static MethodInfo m_GetDefaultEditorLanguage;

        public static SystemLanguage GetDefaultEditorLanguage() =>
            (SystemLanguage) m_GetDefaultEditorLanguage.Invoke(null, null);

        private static PropertyInfo p_currentEditorLanguage;

        public static SystemLanguage currentEditorLanguage
        {
            get => (SystemLanguage) p_currentEditorLanguage.GetValue(null);
            set => p_currentEditorLanguage.SetValue(null, value);
        }

        private static MethodInfo f_GetAvailableEditorLanguages;

        public static SystemLanguage[] GetAvailableEditorLanguages() =>
            (SystemLanguage[]) f_GetAvailableEditorLanguages.Invoke(null, null);

        private static MethodInfo m_GetLocalizedString;

        public static string GetLocalizedString(string original) =>
            (string) m_GetLocalizedString.Invoke(null, new object[] {original});

        private static MethodInfo m_GetLocalizedStringInLang;

        public static string GetLocalizedStringInLang(SystemLanguage lang, string original) =>
            (string) m_GetLocalizedStringInLang.Invoke(null, new object[] {lang, original});

        private static MethodInfo m_GetLocalizationResourceFolder;

        public static string GetLocalizationResourceFolder() =>
            (string) m_GetLocalizationResourceFolder.Invoke(null, null);

        private static MethodInfo m_GetCulture;

        public static string GetCulture(SystemLanguage lang) =>
            (string) m_GetCulture.Invoke(null, null);

        private static MethodInfo m_GetLocalizedStringWithGroupName;

        public static string GetLocalizedStringWithGroupName(string original, string groupName) =>
            (string) m_GetLocalizedStringWithGroupName.Invoke(null, new object[] {original, groupName});

        private static MethodInfo m_GetLocalizedStringWithGroupNameInLang;

        public static string GetLocalizedStringWithGroupNameInLang(
            SystemLanguage lang,
            string original,
            string groupName) =>
            (string) m_GetLocalizedStringWithGroupNameInLang.Invoke(null, new object[] {lang, original, groupName});

        private static MethodInfo m_GetContextGroupName;

        public static string GetContextGroupName() =>
            (string) m_GetContextGroupName.Invoke(null, null);

        private static MethodInfo m_SetContextGroupName;

        public static void SetContextGroupName(string groupName) =>
            m_SetContextGroupName.Invoke(null, new object[] {groupName});

        private static PropertyInfo p_enableEditorLocalization;

        public static bool enableEditorLocalization
        {
            get => (bool) p_enableEditorLocalization.GetValue(null);
            set => p_enableEditorLocalization.SetValue(null, value);
        }

        private static PropertyInfo p_noLocalizationGroupName;

        public static string noLocalizationGroupName
        {
            get => (string) p_noLocalizationGroupName.GetValue(null);
        }

        public static string MarkForTranslation(string value)
        {
            return value;
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            var assembly = typeof(Editor).Assembly;
            var t = assembly.GetType("UnityEditor.LocalizationDatabase");
            m_GetDefaultEditorLanguage =
                t.GetMethod("GetDefaultEditorLanguage", BindingFlags.Static | BindingFlags.Public);
            
            p_currentEditorLanguage =
                t.GetProperty("currentEditorLanguage", BindingFlags.Static | BindingFlags.Public);
            
            f_GetAvailableEditorLanguages =
                t.GetMethod("GetAvailableEditorLanguages", BindingFlags.Static | BindingFlags.Public);
            m_GetLocalizedString =
                t.GetMethod("GetLocalizedString", BindingFlags.Static | BindingFlags.Public);
            m_GetLocalizedStringInLang =
                t.GetMethod("GetLocalizedStringInLang", BindingFlags.Static | BindingFlags.Public);
            m_GetLocalizationResourceFolder =
                t.GetMethod("GetLocalizationResourceFolder", BindingFlags.Static | BindingFlags.Public);
            m_GetCulture =
                t.GetMethod("GetCulture", BindingFlags.Static | BindingFlags.Public);
            m_GetLocalizedStringWithGroupName =
                t.GetMethod("GetLocalizedStringWithGroupName", BindingFlags.Static | BindingFlags.Public);
            m_GetLocalizedStringWithGroupNameInLang =
                t.GetMethod("GetLocalizedStringWithGroupNameInLang", BindingFlags.Static | BindingFlags.Public);
            m_GetContextGroupName =
                t.GetMethod("GetContextGroupName", BindingFlags.Static | BindingFlags.Public);
            m_SetContextGroupName =
                t.GetMethod("SetContextGroupName", BindingFlags.Static | BindingFlags.Public);
            p_enableEditorLocalization =
                t.GetProperty("enableEditorLocalization", BindingFlags.Static | BindingFlags.Public);
            p_noLocalizationGroupName =
                t.GetProperty("noLocalizationGroupName", BindingFlags.Static | BindingFlags.Public);
        }
    }
}