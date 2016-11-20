using System.Collections.Generic;
using System.Linq;
using System.Text;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core
{
  public static class DietHelper
  {
    public static List<Recipe> GetRecipes(this Diet diet)
    {
      return diet.DailyDiets.SelectMany(dailyDiet => dailyDiet.Meals.SelectMany(meal => meal.Receipes)).ToList();
    }

    public static string Code(this Meal meal)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.Append("M:");
      meal.Receipes.ForEach(r => stringBuilder.Append(r.Name + ";"));

      return stringBuilder.ToString();
    }

    public static string Code(this DailyDiet dailyDiet)
    {
      var stringBuilder = new StringBuilder();

      dailyDiet.Meals.ForEach(m => stringBuilder.Append(m.Code()));

      return stringBuilder.ToString();
    }
  }
}
