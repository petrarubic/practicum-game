using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LocalizationLanguage
{
    hr, en
}

public static class ULocalization
{
    private static Dictionary<LocalizationLanguage, Dictionary<string, string>> localizations = new Dictionary<LocalizationLanguage, Dictionary<string, string>>();

    public static Dictionary<string, string> defaultLanguageLocalization;

    private static LocalizationLanguage language = LocalizationLanguage.en;
    public static LocalizationLanguage Language => language;

    public static LocalizationLanguage defaultLanguage;

    //event
    public static event Action LanguageChanged;

    public static void Init(LocalizationLanguage defaultLanguage = LocalizationLanguage.en)
    {
        InitializeLanguages();
        ULocalization.defaultLanguage = defaultLanguage;

        defaultLanguageLocalization = localizations[defaultLanguage];
    }

    public static void SetLanguage(LocalizationLanguage language)
    {
        if (language == ULocalization.language) { return; }

        ULocalization.language = language;
        LanguageChanged?.Invoke();
    }

    public static string LocalizedStringForKey(string key)
    {
        var currentLanguageLocalization = localizations[language];

        currentLanguageLocalization.TryGetValue(key, out string translation);

        if (translation == null || translation.Length == 0)
        {
            Debug.LogWarning("Missing translation :" + key + ":  for : " + language.ToString());
            return defaultLanguageLocalization[key];
        }

        return translation;
    }

    //TODO: JUST A MOCK!!
    private static void InitializeLanguages()
    {
        ////should parse from the file :
        //var path = Application.dataPath + "/Localization" + "LocalizationFile.csv";

        //var arrayOfLines = System.IO.File.ReadAllLines(path);
        ////iterate through array - and split it by "," separator

        Dictionary<string, string> localizationHr = new Dictionary<string, string>();
        Dictionary<string, string> localizationEn = new Dictionary<string, string>();

        localizations[LocalizationLanguage.en] = localizationEn;
        localizations[LocalizationLanguage.hr] = localizationHr;

        localizationEn["button1"] = "English";
        localizationEn["button2"] = "Croatian";
        localizationEn["title"] = "Hello";

        localizationHr["button1"] = "Engleski";
        localizationHr["button2"] = "Hrvatski";
        localizationHr["title"] = "Pozdrav";
    }
}
