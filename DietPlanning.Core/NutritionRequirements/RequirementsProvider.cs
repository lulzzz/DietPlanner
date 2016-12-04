using System;

namespace DietPlanning.Core.NutritionRequirements
{
  public class RequirementsProvider
  {
    public DietRequirements GetRequirements(PersonalData personalData, int numberOfMeals)
    {
      var requirements = new DietRequirements {Calories = GetPpm(personalData)*personalData.Pal};
      var tolerance = 0.1;

      SetNutrientsRanges(requirements);
      SetMealsSplit(requirements, numberOfMeals);
      requirements.CaloriesAllowedRange = GetRangeOf(requirements.Calories, 1 - tolerance, 1 + tolerance);

      return requirements;
    }

    private void SetMealsSplit(DietRequirements requirements, int numberOfMeals)
    {
      //todo: add more meals
      if(numberOfMeals != 5)
        throw new ApplicationException("Only 5 meals supported for now");
      requirements.MealCaloriesSplit.Add(GetRangeOf(requirements.Calories, 0.25, 0.3));
      requirements.MealCaloriesSplit.Add(GetRangeOf(requirements.Calories, 0.05, 0.1));
      requirements.MealCaloriesSplit.Add(GetRangeOf(requirements.Calories, 0.3, 0.35));
      requirements.MealCaloriesSplit.Add(GetRangeOf(requirements.Calories, 0.05, 0.1));
      requirements.MealCaloriesSplit.Add(GetRangeOf(requirements.Calories, 0.15, 0.2));
    }

    private void SetNutrientsRanges(DietRequirements requirements)
    {
      requirements.ProteinRange = GetRangeOf(requirements.Calories/AtwaterFactors.Proteins, 0.1, 0.15);
      requirements.FatRange = GetRangeOf(requirements.Calories/AtwaterFactors.Fat, 0.2, 0.35);
      requirements.CarbohydratesRange = GetRangeOf(requirements.Calories/AtwaterFactors.Carbohydrates, 0.5, 0.7);
    }

    private double GetPpm(PersonalData personalData)
    {
      if (personalData.Gender == Gender.Male)
      {
        return 66.47 + 13.75*personalData.Weight + 5*personalData.Height - 6.75*personalData.Age;
      }

      return 665.09 + 9.56*personalData.Weight + 1.85*personalData.Height - 4.67*personalData.Age;
    }

    private Range GetRangeOf(double value, double lower, double upper)
    {
      return new Range
      {
        Lower = value*lower,
        Upper = value*upper
      };
    }
  }
}
