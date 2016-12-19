using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DietPlanning.NSGA;
using RDotNet;

namespace RAdapter
{
  public static class RInvoker
  {
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
          scoreS.Append(currentIndividualScore.ToString(CultureInfo.InvariantCulture) + ", ");
        }
      }

      var engine = REngine.GetInstance();

      var data = engine.CreateNumericVector(scores.ToArray());
      engine.SetSymbol("data", data);
      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] { individuals.Count }));
      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] { individuals.First().Evaluations.Count }));

      engine.Evaluate(cmd).AsCharacter().ToArray();

      var hv = engine.GetSymbol("dhv").AsNumeric();

      var hvValue = hv.ToArray().First();

      if (Math.Abs(hvValue) < 0.0001)
      {
        var letsTakeABreak = true;
      }

      return hvValue;
    }
  }
}
