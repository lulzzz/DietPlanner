using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DataProviders.Csv;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.MathImplementation;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var nsgaFactory = new NsgaSolverFactory(new Random());
      var recipesProvider = new CsvRecipeProvider(new Random(), "DataProviders/Csv/ingredientsv3.csv");
      var recipes = recipesProvider.GetRecipes();
      var configuration = new ConfigurationProvider().GetConfiguration();
      var personalData = GetPersonalData();

      var solver = nsgaFactory.GetGroupDietSolver(recipes, personalData, configuration);

      var result = solver.Solve();

      result.Fronts.SelectMany(f => f).Select(i => i.Evaluations[0].Score).ToList().OrderBy(s => s).Take(20).ToList().ForEach(Console.WriteLine);
      result.Log.FeasibleSolutions.ForEach(e => Console.Write($"{e}, "));

      Console.WriteLine("done");
      Console.ReadKey();
    }

    private static void LogMathFrontResult(List<List<Individual>> result)
    {
      CsvLogger.RegisterLogger("frontResult");

      foreach (var individual in result.First())
      {
        CsvLogger.AddRow("frontResult",
          new dynamic[]
            {((MathIndividual) individual).X1, individual.Evaluations[0].Score, individual.Evaluations[1].Score});
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
            new dynamic[]
              {i, individual.Evaluations[0].Score, individual.Evaluations[1].Score, individual.Evaluations[2].Score});
        }
      }

      CsvLogger.Write("frontResult", "d:\\FrontResult.csv");
    }

    private static List<PersonalData> GetPersonalData()
    {
      var rp = new RequirementsProvider();
      var pd = new List<PersonalData>
      {
        new PersonalData
          {
            Age = 25,
            Gender = Gender.Male,
            Height = 185,
            Weight = 85,
            Pal = 1.5,
            Id = 0,

          },
          new PersonalData
          {
            Age = 22,
            Gender = Gender.Female,
            Height = 160,
            Weight = 50,
            Pal = 1.8,
            Id = 1
          },
          new PersonalData
          {
            Age = 35,
            Gender = Gender.Male,
            Height = 220,
            Weight = 110,
            Pal = 1.9,
            Id = 2,

          },
          new PersonalData
          {
            Age = 44,
            Gender = Gender.Female,
            Height = 170,
            Weight = 65,
            Pal = 1.3,
            Id = 3
          }
      };

      pd.ForEach(d => d.Requirements = rp.GetRequirements(d, 5));

      return pd;
    }
  }
}
