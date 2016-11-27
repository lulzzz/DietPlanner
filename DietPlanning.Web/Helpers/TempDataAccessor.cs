using System;
using System.Web.Mvc;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Helpers
{
  public static class TempDataAccessor
  {
    const string PersonalDataKey = "PersonalData";
    const string PreferencesKey = "Preferences";
    const string LogKey = "Log";
    const string NsgaResultKey = "NsgaResult";
    const string DailyDietsResultViewModeltKey = "DailyDietsResultViewModel";

    public static DailyDietsResultViewModel GetDailyDietsResultViewModel(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(DailyDietsResultViewModeltKey))
      {
        return null;
      }

      return tempData.Peek(DailyDietsResultViewModeltKey) as DailyDietsResultViewModel;
    }

    public static void SaveDailyDietsResultViewModel(this TempDataDictionary tempData, DailyDietsResultViewModel nsgaResult)
    {
      tempData[DailyDietsResultViewModeltKey] = nsgaResult;
    }

    public static NsgaResult GetNsgaResult(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(NsgaResultKey))
      {
        return null;
      }

      return tempData.Peek(NsgaResultKey) as NsgaResult;
    }

    public static void SaveNsgaResult(this TempDataDictionary tempData, NsgaResult nsgaResult)
    {
      tempData[NsgaResultKey] = nsgaResult;
    }

    public static NsgaLog GetLog(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(LogKey))
      {
        var log = new NsgaLog();
        tempData[LogKey] = log;
      }

      return tempData.Peek(LogKey) as NsgaLog;
    }

    public static void SaveLog(this TempDataDictionary tempData, NsgaLog log)
    {
      tempData[LogKey] = log;
    }

    public static PreferencesViewModel GetPreferences(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(PreferencesKey))
      {
        var preferences = new PreferencesViewModel();
        InitializePrferences(preferences);
        tempData[PreferencesKey] = preferences;
      }

      return tempData.Peek(PreferencesKey) as PreferencesViewModel;
    }

    public static PreferencesViewModel SavePreferences(this TempDataDictionary tempData, PreferencesViewModel preferences)
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

    private static void InitializePrferences(PreferencesViewModel preferences)
    {
      foreach (FoodGroup group in Enum.GetValues(typeof(FoodGroup)))
      {
        preferences.FoodGroupPreferences.Add(new FoodGroupPreference { Group = group, Preference = 0 });
      }

      foreach (RecipeGroup group in Enum.GetValues(typeof(RecipeGroup)))
      {
        preferences.RecipeGroupPreferences.Add(new RecipeGroupPreference { Group = group, Preference = 0 });
      }
    }
  }
}