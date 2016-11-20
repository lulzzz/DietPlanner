using System.Collections.Generic;

namespace DietPlanning.Core.NutritionRequirements
{
  public class DietRequirements
  {
    public double Calories;
    public List<Range> MealCaloriesSplit;
    public Range ProteinRange;
    public Range FatRange;
    public Range CarbohydratesRange;
    public Range CaloriesAllowedRange;
    public double CaloriesTolerance;

    public DietRequirements()
    {
      MealCaloriesSplit = new List<Range>();
    }
  }
}
