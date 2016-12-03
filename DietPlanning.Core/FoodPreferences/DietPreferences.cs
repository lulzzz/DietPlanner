using System.Collections.Generic;

namespace DietPlanning.Core.FoodPreferences
{
  public class DietPreferences
  {
    public List<CategoryPreference> CategoryPreferences;
    public List<string> WordPreferences;
    public List<int> ForbiddenFoods;

    public DietPreferences()
    {
      CategoryPreferences = new List<CategoryPreference>();
      WordPreferences = new List<string>();
      ForbiddenFoods = new List<int>();
    }
  }
}
