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

      var recipes =  diet.Meals.SelectMany(meal => meal.Receipes).ToList();
      var portions = recipes.SelectMany(portion => portion.Ingredients).ToList();

      portions.ForEach(portion => AddNutrientsFromPortion(dietSummary, portion));

      dietSummary.Calories = CalculateCalories(dietSummary);

      return dietSummary;
    }

    private static double CalculateCalories(DietSummary dietSummary)
    {
      return dietSummary.Carbohydrates * AtwaterFactors.Carbohydrates +
             dietSummary.Fat * AtwaterFactors.Fat +
             dietSummary.Proteins * AtwaterFactors.Proteins;
    }

    private static void AddNutrientsFromPortion(DietSummary dietSummary, FoodPortion portion)
    {
      dietSummary.Carbohydrates += portion.Food.Carbohydrates*portion.Amount/100.0;
      dietSummary.Fat += portion.Food.Fat*portion.Amount/100.0;
      dietSummary.Proteins += portion.Food.Proteins*portion.Amount/100.0;
    }
  }
}
