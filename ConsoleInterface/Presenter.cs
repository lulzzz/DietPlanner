using System.Collections.Generic;
using System.IO;
using System.Linq;
using DietPlanning.Core.DomainObjects;

namespace ConsoleInterface
{
  public static class Presenter
  {
    public static void Output(List<Diet> diets)
    {
      var file = new StreamWriter("d:\\test.txt");

      foreach (var diet in diets)
      {
        diet.DailyDiets.ForEach(dailyDiet => Output(dailyDiet, file));
        file.WriteLine();
      }

      file.Close();
    }

    private static void Output(DailyDiet dailyDiet, StreamWriter file)
    {
      var recipes = dailyDiet.Meals.SelectMany(meal => meal.Receipes);

      recipes.ToList().ForEach(recipe => file.Write(recipe.Name.Substring(5)));
    }
  }
}
