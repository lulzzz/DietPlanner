using System.Collections.Generic;
using System.Text;
using DietPlanning.Core.DomainObjects;

namespace Tools
{
  public static class DietTools
  {
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
