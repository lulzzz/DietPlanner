namespace DietPlanning.Core.FoodPreferences
{
  public class CategoryPreference
  {
    public string Name;
    public double Preference;
    public CategoryLevel CategoryLevel;
  }

  public enum CategoryLevel
  {
    MainCategory,
    SubCategory
  }
}