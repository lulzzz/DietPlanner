using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleInterface.Storage;
using DietPlanning.NSGA;
using Newtonsoft.Json;
using RAdapter;
using Storage;

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

      //AggregateOutputsToPareto();
      var referencePareto = LoadReferencePareto();
      NsgaHelper.AssignCrowdingDistances(referencePareto.ResultPoints);
      var filteredIndividuals = referencePareto.ResultPoints.OrderByDescending(i => i.CrowdingDistance).Take(150).ToList();
      var nadir = FindNadir(filteredIndividuals);
      var referenceParetoHyperVolume = RInvoker.HyperVolume(filteredIndividuals, nadir);

      var results = LoadResults(OutputsDirectory);

      var counter = 0;
      var counterMax = results.Count;

      foreach (var result in results)
      {
        result.HyperVolume = referenceParetoHyperVolume - RInvoker.HyperVolume(result.ResultPoints, nadir);
        counter++;
        Console.WriteLine($"{counter}/{counterMax}");
      }

      WriteToCsv(OutputsDirectory, results);
      SaveAverageValues(results);

      Console.WriteLine("done");
      Console.ReadLine();
    }

    private static void SaveAverageValues(List<FrontResult> results)
    {
      var groupped =
        results.GroupBy(
          r =>
            r.PopulationSize.ToString(CultureInfo.InvariantCulture) + "_" +
            r.Iterations.ToString(CultureInfo.InvariantCulture) + "_" +
            r.MutationProbability.ToString(CultureInfo.InvariantCulture)).ToList();

      var csv = new StringBuilder();
      csv.AppendLine("Iterations;MutationProbability;PopulationSize;Time;pvalue;w;HyperVolume;pvalue;w");

      foreach (var group in groupped)
      {
        var hvs = group.Select(i => i.HyperVolume).ToList();
        var times = group.Select(i => i.Time).ToList();
        var avg = new AverageResult();

        if (hvs.Count > 3 && hvs.Any(value => value > 0))
        {
          avg.NormalityHv = RInvoker.Shapiro(hvs);
        }

        if (times.Count > 3 && hvs.Any(value => value > 0))
        {
          avg.NormalityTime = RInvoker.Shapiro(times);
        }
        avg.Hypervolume = group.Select(i => i.HyperVolume).Average();
        avg.Time = group.Select(i => i.Time).Average();
        
        csv.AppendLine(
         $"{group.First().Iterations};{group.First().MutationProbability};{group.First().PopulationSize};{avg.Time};{avg.NormalityTime.Pvalue};{avg.NormalityTime.Statistuc};{avg.Hypervolume};{avg.NormalityHv.Pvalue};{avg.NormalityHv.Statistuc}");
      }

      var filesInOutputDir = Directory.GetFiles(OutputsDirectory);
      var fileIndex = 0;
      var fileName = "averageResults.csv";

      while (filesInOutputDir.Any(path => path.EndsWith(fileName)))
      {
        fileIndex++;
        fileName = $"averageResults_{fileIndex}.csv";
      }

      File.WriteAllText($"{OutputsDirectory}\\{fileName}", csv.ToString());
    }

    private static ResultPoint FindNadir(List<ResultPoint> resultPoints)
    {
      return new ResultPoint
      {
        PreparationTime = resultPoints.Select(rp => rp.PreparationTime).Max(),
        Cost = resultPoints.Select(rp => rp.Cost).Max(),
        Macro = resultPoints.Select(rp => rp.Macro).Max(),
        Preferences = resultPoints.Select(rp => rp.Preferences).Max()
      };
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
        if (individuals.Any(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score > 400))
        {
          continue;
        }
        paretoFronts.Add(NsgaHelper.FindFirstFront(individuals));
      }

      var referenceParetoConverted = NsgaHelper.FindFirstFront(paretoFronts.SelectMany(p => p).ToList()).Select(Mapper.CreateResultPoint);

      SaveAsJson(OutputsDirectory, new FrontResult { ResultPoints = referenceParetoConverted.ToList() });

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
        if (output.Contains("pareto"))
        {
          continue;
        }
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

    public static FrontResult LoadReferencePareto()
    {
      const string filepath = OutputsDirectory + "\\pareto\\pareto.json";

      var json = File.ReadAllText(filepath);

      return JsonConvert.DeserializeObject<FrontResult>(json);
    }

    private static void WriteToCsv(string outputPath, List<FrontResult> results)
    {
      var csv = new StringBuilder();

      csv.AppendLine("SeriesName;Iterations;Machine;MutationProbability;PopulationSize;Time");

      foreach (var testResult in results)
      {
        csv.AppendLine(
          $"{testResult.SeriesName};{testResult.Iterations};{testResult.Machine};{testResult.MutationProbability};{testResult.PopulationSize};{testResult.Time}; {testResult.HyperVolume}");
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

    public static void SaveAsJson(string outputPath, FrontResult testResult)
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
