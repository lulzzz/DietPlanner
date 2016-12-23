using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RAdapter;

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

      var results = LoadResults(OutputsDirectory);
      //WriteToCsv(searchDirectory, results);

      var groupped =
        results.GroupBy(
          r =>
            r.PopulationSize.ToString(CultureInfo.InvariantCulture) + "_" +
            r.Iterations.ToString(CultureInfo.InvariantCulture) + "_" +
            r.MutationProbability.ToString(CultureInfo.InvariantCulture)).ToList();

      var shapiroResults = new List<NormalityResult>();

      foreach (var group in groupped)
      {
        var frontResults = group.ToList();
        var times = frontResults.Select(fr => fr.Time).ToList();

        shapiroResults.Add(RInvoker.Shapiro(times));
      }

      Console.WriteLine("done");
      Console.ReadLine();
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
  }
}
