using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DietPlanning.Core.DataProviders.Csv;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.MathImplementation;
using RAdapter;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    private const string OutputPath = "D:\\Studia\\Informatyka\\praca dyplomowa\\outputs\\";

    public static void Main(string[] args)
    {
      //var nsgaFactory = new NsgaSolverFactory(new Random());
      var recipesProvider = new CsvRecipeProvider(new Random(), "DataProviders/Csv/ingredientsv3.csv");
      var recipes = recipesProvider.GetRecipes();
      var configuration = new ConfigurationProvider().GetConfiguration();
      var personalData = GetPersonalData();

      //var solver = nsgaFactory.GetGroupDietSolver(recipes, personalData, configuration);

      //var result = solver.Solve();

      MutationTests(recipes, personalData, configuration);
      //LogAverage(result);

      
      Console.WriteLine("done");
      Console.ReadKey();
    }

    private static void MutationTests(List<Recipe> recipes, List<PersonalData> personalData, Configuration configuration)
    {
      const string averageValues = "average";
      const string eachIterationValues = "each";

      CsvLogger.RegisterLogger(averageValues);
      CsvLogger.RegisterLogger(eachIterationValues);
      
      CsvLogger.AddRow(averageValues, new dynamic[]
      {
       "MaxIterations" , "PopulationSize", "MutationProbability", "SolvingTime", "hyperVolume"
      });

      var nsgaFactory = new NsgaSolverFactory(new Random());

      const int repeats = 10;
      const double mutationMin = 0.0001;
      const double mutationMax = 0.04;
      const double mutationStep = 0.005;
      
      for (var i = mutationMin; i < mutationMax; i += mutationStep)
      {
        var hvs = new List<double>();
        var times = new List<int>();

        configuration.MutationProbability = i;
       for(var j = 0; j < repeats; j++)
        {
          var solver = nsgaFactory.GetGroupDietSolver(recipes, personalData, configuration);
          var result = solver.Solve();

          hvs.Add(RInvoker.HyperVolume(result.Fronts.SelectMany(f => f).ToList()));
          times.Add(result.Log.SolvingTime);

          Console.WriteLine((double)(j + 1) + "/" + repeats + " current");
        }
        
        Console.WriteLine(((i+ mutationStep) / mutationMax * 100) + "% total");

        var hvsLog = new List<dynamic> {"hvs", configuration.MutationProbability};
        hvsLog.AddRange(hvs.Select(h => (dynamic)h));
        CsvLogger.AddRow(eachIterationValues, hvsLog.ToArray());

        var timeLog = new List<dynamic> { "time", configuration.MutationProbability };
        timeLog.AddRange(times.Select(t => (dynamic)t));
        CsvLogger.AddRow(eachIterationValues, timeLog.ToArray());

        LogAverage(averageValues, configuration, times.Average(), hvs.Average());
      }

      var saveDirectory = OutputPath + "\\" + DateTime.Now.Ticks + "_mutation";

      if (!Directory.Exists(saveDirectory))
      {
        Directory.CreateDirectory(saveDirectory);
      }

      CsvLogger.Write(averageValues, saveDirectory + "\\average.csv");
      CsvLogger.Write(eachIterationValues, saveDirectory + "\\iterations.csv");
    }

    private static void LogAverage(string loggername, Configuration configuration, double avgTime, double avgHv)
    {
      CsvLogger.AddRow(loggername, new dynamic[]
      {
       configuration.MaxIterations ,configuration.PopulationSize, configuration.MutationProbability, avgTime, avgHv
      });     
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
          //new PersonalData
          //{
          //  Age = 35,
          //  Gender = Gender.Male,
          //  Height = 220,
          //  Weight = 110,
          //  Pal = 1.9,
          //  Id = 2,

          //},
          //new PersonalData
          //{
          //  Age = 44,
          //  Gender = Gender.Female,
          //  Height = 170,
          //  Weight = 65,
          //  Pal = 1.3,
          //  Id = 3
          //}
      };
      var preferences = Getpreferences();
      pd.ForEach(d => d.Requirements = rp.GetRequirements(d, 5));
      pd.ForEach(d => d.Preferences = preferences);

      return pd;
    }

    private static DietPreferences Getpreferences()
    {
      var catPreferenres = new List<CategoryPreference>();
      var random = new Random();

      for (int i = 0; i < 10; i++)
      {
        catPreferenres.Add(new CategoryPreference
        {
          SubCategory = RandomEnumValue<SubCategory>(random),
          Preference = 0.5 + random.NextDouble()
        });
      }

      var dietPreferences = new DietPreferences();

      foreach (var preference in catPreferenres)
      {
        if (dietPreferences.CategoryPreferences.Any(p => p.SubCategory == preference.SubCategory))
        {
          continue;
        }

        dietPreferences.CategoryPreferences.Add(preference);
      }

      return dietPreferences;
    }

    static T RandomEnumValue<T>(Random random)
    {
      var v = Enum.GetValues(typeof(T));
      return (T)v.GetValue(random.Next(v.Length));
    }
  }
}
