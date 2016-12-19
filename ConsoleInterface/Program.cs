using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleInterface.Storage;
using DietPlanning.Core.DataProviders.Csv;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using RAdapter;
using Tools;
using Random = System.Random;

namespace ConsoleInterface
{
  public class Program
  {
    private const string OutputPath = "D:\\Studia\\Informatyka\\praca dyplomowa\\outputs\\";
    private const string dateFormat = "-yyyy-MM-dd_HH-mm-ss";

    public static void Main(string[] args)
    {
      //args = new[] { "population", "100", "150", "25", "2", "33" };
      //var nsgaFactory = new NsgaSolverFactory(new Random());
      var recipesProvider = new CsvRecipeProvider(new Random(), "DataProviders/Csv/ingredientsv3.csv");
      var recipes = recipesProvider.GetRecipes();
      var personalData = GetPersonalData();
      var repeats = int.Parse(args[4]);
      var timeBound = int.Parse(args[5]);
      string seriesName;

      var configurations = CreateConfigurations(args, out seriesName);
      seriesName += "_" + repeats + DateTime.Now.ToString(dateFormat);

      var totalSteps = configurations.Count*repeats;
      var currentStep = 0;

      foreach (var configuration in configurations)
      {
        
        for (int j = 0; j < repeats; j++)
        {
          NsgaResult result = null;

          while (result == null)
          {
            result = RunConfigurationWithTimer(timeBound, configuration, recipes, personalData);
          }
          currentStep++;
          Console.WriteLine((double)currentStep / totalSteps + "%  " + (double)result.Log.SolvingTime/1000);
          var testResult = StorageHelper.CreatTestResult(result, configuration);
          StorageHelper.SaveAsJson(OutputPath, seriesName, testResult);
        }
      }

      Console.WriteLine("done");
      Console.ReadKey();
    }

    private static List<Configuration> CreateConfigurations(string[] args, out string seriesName)
    {
      var configurationProvider = new ConfigurationProvider();
      var type = args[0];
      var configurations = new List<Configuration>();
      seriesName = "";

      if (type == "mutation")
      {
        var min = double.Parse(args[1], CultureInfo.InvariantCulture);
        var max = double.Parse(args[2], CultureInfo.InvariantCulture);
        var step = double.Parse(args[3], CultureInfo.InvariantCulture);

        seriesName = (type + "_" + min + "_" + max + "_" + step).Replace(",","").Replace(".","");

        for (var i = min; i <= max; i+= step)
        {
          var config = configurationProvider.GetConfiguration();
          config.MutationProbability = i;
          configurations.Add(config);
        }
      }
      else if (type == "iterations" || type == "population")
      {
        var min = int.Parse(args[1]);
        var max = int.Parse(args[2]);
        var step = int.Parse(args[3]);

        seriesName = type + "_" + min + "_" + max + "_" + step;

        for (var i = min; i <= max; i += step)
        {
          var config = configurationProvider.GetConfiguration();
         
          if (type == "iterations")
          {
            config.MaxIterations = i;
          }
          else
          {
            config.PopulationSize = i;
          }
          configurations.Add(config);
        }
      }

      return configurations;
    }

    private static NsgaResult RunConfigurationWithTimer(int maxSeconnds, Configuration configuration, List<Recipe> recipes, List<PersonalData> personalData)
    {
      NsgaResult result = new NsgaResult();
      var failed = false;
      var watch = new Stopwatch();

      var workerThread = new Thread(() =>
      {
        try
        {
          watch.Start();
          result = RunConfiguration(configuration, recipes, personalData);
          watch.Stop();
        }
        catch (Exception ex)
        {
          watch.Stop();
          failed = true;
          Console.WriteLine("Aborted");
        }
      });

      var timeoutTimer = new Timer(s =>
      {
        workerThread.Abort();
      }, null, TimeSpan.FromSeconds(maxSeconnds), TimeSpan.FromDays(1));

      workerThread.Start();
      workerThread.Join();

      return failed ? null : result;
    }
    
    private static NsgaResult RunConfiguration(Configuration configuration, List<Recipe> recipes, List<PersonalData> personalData)
    {
      var nsgaFactory = new NsgaSolverFactory(new Random());

      var solver = nsgaFactory.GetGroupDietSolver(recipes, personalData, configuration);
      return solver.Solve();
    }

    private static void Ts(out NsgaResult res)
    {
      res = new NsgaResult();
    }

    private static void MutationTests(List<Recipe> recipes, List<PersonalData> personalData, Configuration configuration, double min, double max, double step, int repeats)
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
      
      for (var i = min; i < max; i += step)
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
        
        Console.WriteLine(((i+ step) / max * 100) + "% total");

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
