using System;
using System.Linq;

using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Genetic;
using DietPlanning.NSGA;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var solver = new NsgaSolver(new Sorter());

      var recipeGenerator = new RandomRecipeProvider(new Random());

      var recipes = recipeGenerator.GetRecipes(100, new FoodDatabaseProvider().GetFoods());

      Console.WriteLine(recipes.Count);

      Console.ReadKey();
    }

    private static DietSummary GetTargetDiet()
    {
      return new DietSummary
      {
        Calories = 3002,
        Fat = 101,
        Carbohydrates = 338,
        Proteins = 188
      };
    }
  }
}
