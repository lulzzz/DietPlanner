using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DietPlanning.NSGA;
using RDotNet;

namespace RAdapter
{
  public static class RInvoker
  {
    public static void UseR()
    {
      var engine = REngine.GetInstance();

      var directory = Environment.CurrentDirectory;
      var rFilePath = directory + "\\hv.r";
      var cmd = $"source('{rFilePath}')".Replace("\\","/");

      Console.WriteLine(cmd);
      Console.WriteLine(rFilePath);

      string[] a = engine.Evaluate(cmd).AsCharacter().ToArray();

      Console.Write(a);

      engine.Dispose();
    }

    public static double HyperVolume(List<Individual> individuals)
    {
      var directory = Environment.CurrentDirectory;
      var rFilePath = directory + "\\hv.r";
      var cmd = $"source('{rFilePath}')".Replace("\\", "/");

      var scores = new List<double>();

      var scoreS = new StringBuilder();
      foreach (var individual in individuals)
      {
        var currentIndividualScores = individual.Evaluations.Select(e => e.Score).ToList();
        scores.AddRange(currentIndividualScores);
        foreach (var currentIndividualScore in currentIndividualScores)
        {
          scoreS.Append(currentIndividualScore.ToString(CultureInfo.InvariantCulture)+", ");
        }
      }


      File.WriteAllText("output.txt", scoreS.ToString());

      var engine = REngine.GetInstance();

      var data = engine.CreateNumericVector(scores.ToArray());
      engine.SetSymbol("data", data);
      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] {individuals.Count}));
      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] {individuals.First().Evaluations.Count}));

      engine.Evaluate(cmd).AsCharacter().ToArray();

      var hv = engine.GetSymbol("dhv").AsNumeric();

      return hv.ToArray().First();
    }
  }
}
