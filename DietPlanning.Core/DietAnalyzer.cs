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

      dietSummary.Carbohydrates += mealSumamry.Carbohydrates;
      dietSummary.Fat += mealSumamry.Fat;
      dietSummary.Proteins += mealSumamry.Proteins;
      dietSummary.Calories += mealSumamry.Calories;

      dietSummary.CaloriesPerMeal.Add(mealSumamry.Calories);
    }

    private DietSummary CreateMealSummary(Meal meal)
    {
      var mealSummary = new DietSummary();
      //var portions = meal.Receipes.SelectMany(portion => portion.Ingredients).ToList();
      //portions.ForEach(portion => AddNutrientsFromPortion(mealSummary, portion));
      //mealSummary.Calories = CalculateCalories(mealSummary);

      var nutritionValues = meal.Receipes.Select(r => r.NutritionValues).ToList();
      nutritionValues.ForEach(v => AddNutritionValues(mealSummary, v));

      return mealSummary;
    }

    private void AddNutritionValues(DietSummary mealSummary, NutritionValues nutritionValues)
    {
      mealSummary.Carbohydrates += nutritionValues.Carbohydrates;
      mealSummary.Fat += nutritionValues.Fat;
      mealSummary.Proteins += nutritionValues.Proteins;
      mealSummary.Calories += nutritionValues.Calories;
    }


    //private void AddNutrientsFromPortion(DietSummary mealSummary, FoodPortion portion)
    //{
    //  mealSummary.Carbohydrates += portion.Food.Carbohydrates * portion.Amount / 100.0;
    //  mealSummary.Fat += portion.Food.Fat * portion.Amount / 100.0;
    //  mealSummary.Proteins += portion.Food.Proteins * portion.Amount / 100.0;
    //}

    private double CalculateCalories(DietSummary dietSummary)
    {
      return dietSummary.Carbohydrates * AtwaterFactors.Carbohydrates +
             dietSummary.Fat * AtwaterFactors.Fat +
             dietSummary.Proteins * AtwaterFactors.Proteins;
    }
  }
}
