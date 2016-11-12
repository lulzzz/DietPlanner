﻿using System;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DietImplementation;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CsvLogger.Init();

      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var nsgaSolverFactory = new NsgaSolverFactory(new ConfigurationProvider(), new Random());
      var nsgaSolver = nsgaSolverFactory.GetDietSolver(recipeGenerator.GetRecipes(), GetTargetDiet());
      var mathNsgaSolver = nsgaSolverFactory.GetMathSolver();

      var result = mathNsgaSolver.Solve();
      
      CsvLogger.Write("d:\\output.csv");

      Console.WriteLine("done");
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
