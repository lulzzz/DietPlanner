using DietPlanning.Core.FoodPreferences;

namespace DietPlanning.Core.NutritionRequirements
{
  public class PersonalData
  {
    public int Age { get; set; }
    public int Weight { get; set; }
    public int Height { get; set; }
    public int Id { get; set; }
    public double Pal { get; set; }
    public Gender Gender { get; set; }
    public DietRequirements Requirements { get; set; }
    public DietPreferences Preferences { get; set; }

    public PersonalData()
    {
      Preferences = new DietPreferences();
    }
  }
}
