using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core
{
  public static class DietHelper
  {
    public static List<Recipe> GetRecipes(this Diet diet)
    {
      return diet.DailyDiets.SelectMany(dailyDiet => dailyDiet.Meals.SelectMany(meal => meal.Receipes)).ToList();
    }
  }
}
