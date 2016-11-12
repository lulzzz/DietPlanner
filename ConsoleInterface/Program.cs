﻿using System;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.NSGA;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      const int tournamentSize = 2;

      var configProvider = new ConfigurationProvider();
      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var recipes = recipeGenerator.GetRecipes();

      var nsgaSolver = new NsgaSolver(
        new Sorter(), 
        new PopulationInitializer(new Random(), recipes),
        new Evaluator(new DietAnalyzer()),
        new TournamentSelector(new CrowdedDistanceComparer(), tournamentSize, new Random()),
        new DayCrossOver(new Random()),
        new Mutator(new Random(), recipes),
        configProvider.GetConfiguration());

      CsvLogger.Init();

      var result = nsgaSolver.Solve(GetTargetDiet());
      
      CsvLogger.Write("d:\\output.csv");

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
