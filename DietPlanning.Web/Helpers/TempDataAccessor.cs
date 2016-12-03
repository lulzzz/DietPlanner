using System;
using System.Web.Mvc;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Helpers
{
  public static class TempDataAccessor
  {
    private const string PersonalDataKey = "PersonalData";
    private const string PreferencesKey = "Preferences";
    private const string LogKey = "Log";
    private const string NsgaResultKey = "NsgaResult";
    private const string DailyDietsResultViewModeltKey = "DailyDietsResultViewModel";
    private const string SettingsKey = "Settings";
    private const string DietPreferencesKey = "DietPreferencesKey";

    public static SettingsViewModel GetSettings(this TempDataDictionary tempData)
    {
      if (tempData.ContainsKey(DailyDietsResultViewModeltKey)) return tempData.Peek(SettingsKey) as SettingsViewModel;

      var configurationProvider = new ConfigurationProvider();
      var settingsViewModel = new SettingsViewModel { NsgaConfiguration = configurationProvider.GetConfiguration() };
      tempData.SaveSettings(settingsViewModel);
      return tempData.Peek(SettingsKey) as SettingsViewModel;
    }

    public static DietPreferences GetDietPreferences(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(DietPreferencesKey) ? tempData.Peek(DietPreferencesKey) as DietPreferences : new DietPreferences();
    }

    public static void SaveDietPreferences(this TempDataDictionary tempData, DietPreferences dietPreferences)
    {
      tempData[DietPreferencesKey] = dietPreferences;
    }

    public static void SaveSettings(this TempDataDictionary tempData, SettingsViewModel settings)
    {
      tempData[SettingsKey] = settings;
    }

    public static DailyDietsResultViewModel GetDailyDietsResultViewModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(DailyDietsResultViewModeltKey) ? tempData.Peek(DailyDietsResultViewModeltKey) as DailyDietsResultViewModel : null;
    }

    public static void SaveDailyDietsResultViewModel(this TempDataDictionary tempData, DailyDietsResultViewModel nsgaResult)
    {
      tempData[DailyDietsResultViewModeltKey] = nsgaResult;
    }

    public static NsgaResult GetNsgaResult(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(NsgaResultKey) ? tempData.Peek(NsgaResultKey) as NsgaResult : null;
    }

    public static void SaveNsgaResult(this TempDataDictionary tempData, NsgaResult nsgaResult)
    {
      tempData[NsgaResultKey] = nsgaResult;
    }

    public static NsgaLog GetLog(this TempDataDictionary tempData)
    {
      if (tempData.ContainsKey(LogKey)) return tempData.Peek(LogKey) as NsgaLog;

      var log = new NsgaLog();
      tempData[LogKey] = log;

      return tempData.Peek(LogKey) as NsgaLog;
    }

    public static void SaveLog(this TempDataDictionary tempData, NsgaLog log)
    {
      tempData[LogKey] = log;
    }

    public static PreferencesViewModel GetPreferencesViewModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(PreferencesKey) ? tempData.Peek(PreferencesKey) as PreferencesViewModel : null;
    }

    public static PreferencesViewModel SavePreferencesViewModel(this TempDataDictionary tempData, PreferencesViewModel preferences)
    {
      tempData[PreferencesKey] = preferences;

      return preferences;
    }

    public static PersonalData GetPersonalData(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(PersonalDataKey))
      {
        tempData[PersonalDataKey] = new PersonalData
        {
          Age = 25,
          Gender = Gender.Male,
          Height = 185,
          Weight = 85,
          Pal = 1.5
        };
      }

      return tempData.Peek(PersonalDataKey) as PersonalData;
    }

    public static void SavePersonalData(this TempDataDictionary tempData, PersonalData personalData)
    {
      tempData[PersonalDataKey] = personalData;
    }
  }
}