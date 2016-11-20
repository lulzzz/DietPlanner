﻿using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.NSGA.DietImplementation;
using DietPlanning.NSGA.MathImplementation;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CsvLogger.RegisterLogger("iterationEvaluations");
      

      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var nsgaSolverFactory = new NsgaSolverFactory(new ConfigurationProvider(), new Random());
      var recipes = recipeGenerator.GetRecipes();
      var requirementsProvider = new RequirementsProvider();
      // var nsgaSolver = nsgaSolverFactory.GetDietSolver(recipes, GetTargetDiet());
      // var mathNsgaSolver = nsgaSolverFactory.GetMathSolver();
      var dietRequirements = requirementsProvider.GetRequirements(GetPersonalData(), 5);

      var dailyDietsNsgaSolver = nsgaSolverFactory.GetDailyDietsSolver(recipes, dietRequirements);

      var result = dailyDietsNsgaSolver.Solve();

      //LogMathFrontResult(result);
      LogDailyDietsFrontResult(result);

      CsvLogger.Write("iterationEvaluations", "d:\\output.csv");
      
      Console.WriteLine("done");
      Console.ReadKey();
     }

    private static void LogMathFrontResult(List<List<Individual>> result)
    {
      CsvLogger.RegisterLogger("frontResult");

      foreach (var individual in result.First())
      {
        CsvLogger.AddRow("frontResult",
          new dynamic[] {((MathIndividual) individual).X1, individual.Evaluations[0].Score, individual.Evaluations[1].Score});
      }
      
      CsvLogger.Write("frontResult", "d:\\FrontResult.csv");
    }

    private static void LogDailyDietsFrontResult(List<List<Individual>> result)
    {
      CsvLogger.RegisterLogger("frontResult");

      for (var i = 0; i < result.Count; i++)
      {
        foreach (var individual in result[i])
        {
          CsvLogger.AddRow("frontResult",
            new dynamic[] {i, individual.Evaluations[0].Score, individual.Evaluations[1].Score, individual.Evaluations[2].Score });
        }
      }
      
      CsvLogger.Write("frontResult", "d:\\FrontResult.csv");
    }

    private static PersonalData GetPersonalData()
    {
      return new PersonalData
      {
        Age = 25,
        Gender = Gender.Male,
        Height = 185,
        Weight = 85,
        Pal = 1.5
      };
    }
  }
}
