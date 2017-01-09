using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.Web.Models;
using MultiAttributeDecisionMaking;

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
    private const string PersonalDataListKey = "PersonalDataListKey";
    private const string GroupDietsResultViewModeltKey = "GroupDietsResultViewModeltKey";
    private const string AhpKey = "Pairwise";
    private const string TopsisKey = "Point";
    private const string PrefPointKey = "PrefPointKey";

    public static void SaveAhpModel(this TempDataDictionary tempData, AhpModel ahpModel)
    {
      tempData[AhpKey] = ahpModel;
    }

    public static AhpModel GetAhpModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(AhpKey) ? tempData.Peek(AhpKey) as AhpModel : new AhpModel();
    }

    public static void SaveTopsisModel(this TempDataDictionary tempData, WeightsModel weightsModel)
    {
      tempData[TopsisKey] = weightsModel;
    }

    public static WeightsModel GetTopsisModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(TopsisKey) ? tempData.Peek(TopsisKey) as WeightsModel : new WeightsModel();
    }

    public static void SavePreferencePointModel(this TempDataDictionary tempData, WeightsModel weightsModel)
    {
      tempData[PrefPointKey] = weightsModel;
    }

    public static WeightsModel GetPreferencePointModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(PrefPointKey) ? tempData.Peek(PrefPointKey) as WeightsModel : new WeightsModel();
    }

    public static SettingsViewModel GetSettings(this TempDataDictionary tempData)
    {
      if (tempData.ContainsKey(SettingsKey)) return tempData.Peek(SettingsKey) as SettingsViewModel;

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

    public static GroupDietsResultViewModel GetGroupDietsResultViewModel(this TempDataDictionary tempData)
    {
      return tempData.ContainsKey(GroupDietsResultViewModeltKey) ? tempData.Peek(GroupDietsResultViewModeltKey) as GroupDietsResultViewModel : null;
    }

    public static void SaveGroupDietsResultViewModel(this TempDataDictionary tempData, GroupDietsResultViewModel nsgaResult)
    {
      tempData[GroupDietsResultViewModeltKey] = nsgaResult;
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
          Name = "Stefan",
          Age = 25,
          Gender = Gender.M,
          Height = 185,
          Weight = 85,
          Pal = 1.5
        };
      }

      return tempData.Peek(PersonalDataKey) as PersonalData;
    }

    public static void SavePersonalDataList(this TempDataDictionary tempData, List<PersonalData> personalDatas)
    {
      tempData[PersonalDataListKey] = personalDatas;
    }

    public static List<PersonalData> GetPersonalDataList(this TempDataDictionary tempData)
    {
      if (!tempData.ContainsKey(PersonalDataListKey))
      {
        tempData[PersonalDataListKey] = new List<PersonalData>
        {
          new PersonalData
          {
            Name = "Johny",
            Age = 25,
            Gender = Gender.M,
            Height = 185,
            Weight = 85,
            Pal = 1.5,
            Id = 0,
          },
          new PersonalData
          {
            Name = "Anna",
            Age = 22,
            Gender = Gender.K,
            Height = 160,
            Weight = 50,
            Pal = 1.8,
            Id = 1
          }
        };
      }

      return tempData.Peek(PersonalDataListKey) as List<PersonalData>;
    }

    public static void SavePersonalData(this TempDataDictionary tempData, PersonalData personalData)
    {
      tempData[PersonalDataKey] = personalData;
    }
  }
}