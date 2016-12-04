using System.Linq;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.GroupDiets
{
  public class GroupDietAnalyzer
  {
    public DietSummary SummarizeForPerson(GroupDiet diet, int personId)
    {
      var dietSummary = new DietSummary();

      foreach (var meal in diet.Meals)
      {
        AddMealSummary(dietSummary, meal, personId);
      }

      return dietSummary;
    }

    private void AddMealSummary(DietSummary dietSummary, GroupMeal meal, int personId)
    {
      var mealSumamry = CreateMealSummary(meal, personId);

      dietSummary.NutritionValues += mealSumamry;

      dietSummary.CaloriesPerMeal.Add(mealSumamry.Calories);
    }

    private NutritionValues CreateMealSummary(GroupMeal meal, int personId)
    {
      var mealSummary = new NutritionValues();
      var recipes = meal.GetRecipesForPerson(personId);
      var nutritionValues = recipes.Select(r => r.Item2.NutritionValues*(r.Item1*r.Item2.NominalWeight/100.0)).ToList();

      nutritionValues.ForEach(v => mealSummary += v);

      return mealSummary;
    }
  }
}
