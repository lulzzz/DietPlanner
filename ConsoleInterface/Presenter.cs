using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DietPlanning.Core;

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
    }

    private static void Output(DailyDiet dailyDiet, StreamWriter file)
    {
      var portions = dailyDiet.Meals.SelectMany(meal => meal.FoodPortions);

      portions.ToList().ForEach(portion => file.Write(portion.Food.Id + " "));
    }
  }
}
