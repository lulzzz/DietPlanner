using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleInterface.Storage;
using DietPlanning.NSGA;
using Newtonsoft.Json;

namespace Aggregator
{
  class Program
  {
    const string OutputsDirectory = @"D:\Studia\Informatyka\praca dyplomowa\outputs";
    
    static void Main(string[] args)
    {
      if (!Directory.Exists(OutputsDirectory))
      {
        Console.WriteLine($"{OutputsDirectory} does not exist");
      }
      
      AggregateOutputsToPareto();

      Console.ReadLine();
    }

    private static void AggregateOutputsToPareto()
    {
      var results = LoadResults(OutputsDirectory);

      var groupped =
        results.GroupBy(
          r =>
            r.PopulationSize.ToString(CultureInfo.InvariantCulture) + "_" +
            r.Iterations.ToString(CultureInfo.InvariantCulture) + "_" +
            r.MutationProbability.ToString(CultureInfo.InvariantCulture)).ToList();

      var paretoFronts = new List<List<Individual>>();

      foreach (var group in groupped)
      {
        var individuals = ExtractIndividuals(group.ToList());

        paretoFronts.Add(NsgaHelper.FindFirstFront(individuals));
      }

      var referenceParetoConverted = NsgaHelper.FindFirstFront(paretoFronts.SelectMany(p => p).ToList()).Select(Mapper.CreateResultPoint);

      SaveAsJson(OutputsDirectory, new TestResult { ResultPoints = referenceParetoConverted.ToList() });

      Console.WriteLine("Pareto aggregated");
    }

    private static List<Individual> ExtractIndividuals(List<FrontResult> frontResults)
    {
     var correctResultPoints = frontResults.SelectMany(r => r.ResultPoints).Where(rp =>
     !double.IsNaN(rp.Cost) &&
     !double.IsNaN(rp.PreparationTime) &&
     !double.IsNaN(rp.Preferences) &&
     !double.IsNaN(rp.Macro));

      return correctResultPoints.Select(Mapper.ToIndividual).ToList();
    }

    private static List<FrontResult> LoadResults(string rootPath)
    {
      var results = new List<FrontResult>();

      var outputs = Directory.GetFiles(rootPath, "*.json", SearchOption.AllDirectories);

      foreach (var output in outputs)
      {
        var json = File.ReadAllText(output);
        var testResult = JsonConvert.DeserializeObject<FrontResult>(json);

        if (outputs.First().Contains(@"\Az\"))
        {
          testResult.Machine = "azure";
        }

        results.Add(testResult);
      }

      return results;
    }

    private static void WriteToCsv(string outputPath, List<FrontResult> results)
    {
      var csv = new StringBuilder();

      csv.AppendLine("SeriesName;Iterations;Machine;MutationProbability;PopulationSize;Time");

      foreach (var testResult in results)
      {
        csv.AppendLine(
          $"{testResult.SeriesName};{testResult.Iterations};{testResult.Machine};{testResult.MutationProbability};{testResult.PopulationSize};{testResult.Time}");
      }

      var filesInOutputDir = Directory.GetFiles(outputPath);
      var fileIndex = 0;
      var fileName = "aggregatedResult.csv";

      while (filesInOutputDir.Any(path => path.EndsWith(fileName)))
      {
        fileIndex++;
        fileName = $"aggregatedResult_{fileIndex}.csv";
      }

      File.WriteAllText($"{outputPath}\\{fileName}" , csv.ToString());
    }

    public static void SaveAsJson(string outputPath, TestResult testResult)
    {
      var json = JsonConvert.SerializeObject(testResult);

      var saveDirectory = outputPath + "\\pareto\\";

      if (!Directory.Exists(saveDirectory))
      {
        Directory.CreateDirectory(saveDirectory);
      }

      File.WriteAllText(saveDirectory + "\\pareto" + ".json", json);
    }
  }
}
