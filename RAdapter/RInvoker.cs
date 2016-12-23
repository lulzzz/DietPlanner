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
      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] {individuals.Count}));
      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] {individuals.First().Evaluations.Count}));

      var hv = engine.GetSymbol("dhv").AsNumeric();

      var hvValue = hv.ToArray().First();

      if (Math.Abs(hvValue) < 0.0001)
      {
        var letsTakeABreak = true;
      }

      return hvValue;
    }

    public static NormalityResult Shapiro(List<double> values)
    {
      var engine = REngine.GetInstance();

      var data = engine.CreateNumericVector(values.ToArray());
      engine.SetSymbol("data", data);

      RunScript("Shapiro", engine);

      var pValue = engine.GetSymbol("pv").AsNumeric().ToArray()[0];
      var statistic = engine.GetSymbol("w").AsNumeric().ToArray()[0]; 

      return new NormalityResult { Pvalue = pValue, Statistuc = statistic };
    }

    private static void RunScript(string scriptname, REngine engine)
    {
      var directory = Environment.CurrentDirectory;
      var rFilePath = directory + $"\\Rscripts\\{scriptname}.R";
      var cmd = $"source('{rFilePath}')".Replace("\\", "/");

      engine.Evaluate(cmd);
    }
  }
}
