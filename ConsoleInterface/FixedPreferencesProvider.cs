using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;

namespace ConsoleInterface
{
  public static class FixedPreferencesProvider
  {
    public static List<CategoryPreference> GetPreferences()
    {
      return new List<CategoryPreference>
      {
        new CategoryPreference
        {
          SubCategory = SubCategory.PieczywoPszenne,
          Preference = 0.9
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.Zupy,
          Preference = 1.3
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.PotrawyZWolowiny,
          Preference = 1.34
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.PotrawyDrobiowe,
          Preference = 1.1
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.ProduktyObiadowe,
          Preference = 1.05
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.PotrawyZJaj,
          Preference = 0.76
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.Surowki,
          Preference = 0.95
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.PotrawyRybne,
          Preference = 1.09
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.PotrawyZGrzybow,
          Preference = 0.9
        },
        new CategoryPreference
        {
          SubCategory = SubCategory.LodyMlecznoOwocowe,
          Preference = 0.63
        },
      };
    }
  }
}
