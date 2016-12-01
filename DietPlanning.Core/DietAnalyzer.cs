using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core
{
  public class DietAnalyzer
  {
    public List<DietSummary> Summarize(Diet diet)
    {
      return diet.DailyDiets.Select(SummarizeDaily).ToList();
    }

    public DietSummary SummarizeDaily(DailyDiet diet)
    {
      var dietSummary = new DietSummary();

      foreach (var meal in diet.Meals)
      {
        AddMealSummary(dietSummary, meal);
      }

      return dietSummary;
    }

    private void AddMealSummary(DietSummary dietSummary, Meal meal)
    {
      var mealSumamry = CreateMealSummary(meal);

      dietSummary.NutritionValues += mealSumamry;

      dietSummary.CaloriesPerMeal.Add(mealSumamry.Calories);
    }

    private NutritionValues CreateMealSummary(Meal meal)
    {
      var mealSummary = new NutritionValues();
      
      var nutritionValues = meal.Receipes.Select(r => r.NutritionValues).ToList();

      nutritionValues.ForEach(v => mealSummary += v);

      return mealSummary;
    }
  }
}
