using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;
using RDotNet;
using Storage;

namespace RAdapter
{
  public static class RInvoker
  {
    public static string Path = "";

    public static List<GroupDietIndividual> Topsis(List<GroupDietIndividual> individuals, double costWeight, double prefrencesWeight, double prepTimeWeight, double macroWeight)
    {
      var engine = REngine.GetInstance();
      engine.Initialize();
      engine.ClearGlobalEnvironment();

      var scores = new List<double>();

      foreach (var individual in individuals)
      {
        scores.Add(individual.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score);
        scores.Add(individual.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score);
        scores.Add(individual.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score);
        scores.Add(individual.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score);
      }
      var data = engine.CreateNumericVector(scores.ToArray());

      engine.SetSymbol("data", data);
      engine.SetSymbol("individualsCount", engine.CreateNumericVector(new double[] { individuals.Count }));
      engine.SetSymbol("weights", engine.CreateNumericVector(new[] { costWeight, prefrencesWeight, prepTimeWeight, macroWeight }));
      engine.SetSymbol("impacts", engine.CreateCharacterVector(new[] { "-", "-", "-", "-" }));

      RunScript("Topsis", engine);

      var topsisScores = engine.GetSymbol("scores").AsNumeric().ToList();

      var result = new List<Tuple<int, double>>();
      for (int i = 0; i < topsisScores.Count; i++)
      {
        result.Add(new Tuple<int, double>(i, topsisScores[i]));
      }

      var orderedIndividuals = result.OrderByDescending(r => r.Item2).Select(r => individuals[r.Item1]).ToList();
      
      return orderedIndividuals;
    }


    public static double HyperVolume(List<Individual> individuals, ResultPoint resultPoint = null)
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

      if (resultPoint != null)
      {
        //todo finish
      }

      var engine = REngine.GetInstance();

      var data = engine.CreateNumericVector(scores.ToArray());
      engine.SetSymbol("data", data);
      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] {individuals.Count}));
      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] {individuals.First().Evaluations.Count}));

      RunScript("hv", engine);

      var hv = engine.GetSymbol("dhv").AsNumeric();

      var hvValue = hv.ToArray().First();

      if (Math.Abs(hvValue) < 0.0001)
      {
        var letsTakeABreak = true;
      }

      return hvValue;
    }

    public static double HyperVolume(List<ResultPoint> resultPoints, ResultPoint referencePoint = null)
    {
      var scores = new List<double>();
      var engine = REngine.GetInstance();
      engine.ClearGlobalEnvironment();

      foreach (var point in resultPoints)
      {
        scores.Add(point.Macro);
        scores.Add(point.PreparationTime);
        scores.Add(point.Cost);
        scores.Add(point.Preferences);
      }

      if (referencePoint != null)
      {
        engine.SetSymbol("useReferecnePoint", engine.CreateLogical(true));
        engine.SetSymbol("referencePoint", engine.CreateNumericVector(new []
        {
          referencePoint.Macro, referencePoint.PreparationTime, referencePoint.Cost, referencePoint.Preferences
        }));
      }
      else
      {
        engine.SetSymbol("useReferecnePoint", engine.CreateLogical(false));
      }

      engine.SetSymbol("data", engine.CreateNumericVector(scores.ToArray()));
      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] { resultPoints.Count }));
      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] { 4 }));

      RunScript("hv", engine);

      var hv = engine.GetSymbol("dhv").AsNumeric().ToArray().First();

      return hv;
    }

    public static double Epsilon(List<ResultPoint> resultPoints, List<ResultPoint> trueFront)
    {
      var engine = REngine.GetInstance();

      var data = new List<double>();
      var dataTf = new List<double>();

      foreach (var point in resultPoints)
      {
        data.Add(point.Macro);
        data.Add(point.PreparationTime);
        data.Add(point.Cost);
        data.Add(point.Preferences);
      }

      foreach (var point in trueFront)
      {
        dataTf.Add(point.Macro);
        dataTf.Add(point.PreparationTime);
        dataTf.Add(point.Cost);
        dataTf.Add(point.Preferences);
      }

      engine.SetSymbol("data", engine.CreateNumericVector(data.ToArray()));
      engine.SetSymbol("data_tf", engine.CreateNumericVector(dataTf.ToArray()));

      engine.SetSymbol("individuals", engine.CreateNumericVector(new double[] { resultPoints.Count }));
      engine.SetSymbol("individuals_tf", engine.CreateNumericVector(new double[] { trueFront.Count }));

      engine.SetSymbol("criterions", engine.CreateNumericVector(new double[] { 4 }));
      engine.SetSymbol("criterions_tf", engine.CreateNumericVector(new double[] { 4 }));

      RunScript("Epsilon", engine);

      var epsilon = engine.GetSymbol("eps").AsNumeric().ToArray().First();

      return epsilon;
    }

    public static NormalityResult Shapiro(List<double> values)
    {
      var engine = REngine.GetInstance();
      engine.ClearGlobalEnvironment();

      var data = engine.CreateNumericVector(values.ToArray());
      engine.SetSymbol("data", data);

      RunScript("Shapiro", engine);

      var pValue = engine.GetSymbol("pv").AsNumeric().ToArray()[0];
      var statistic = engine.GetSymbol("w").AsNumeric().ToArray()[0]; 

      //engine.Dispose();

      return new NormalityResult { Pvalue = pValue, Statistic = statistic };
    }

    private static void RunScript(string scriptname, REngine engine)
    {
      if (string.IsNullOrEmpty(Path))
      {
        var directory = Environment.CurrentDirectory;
        var rFilePath = directory + $"\\Rscripts\\{scriptname}.R";
        var cmd = $"source('{rFilePath}')".Replace("\\", "/");
        engine.Evaluate(cmd);
      }
      else
      {
        var cmd = $"source('{Path}\\{scriptname}.R')".Replace("\\", "/");
        engine.Evaluate(cmd);
      }
    }
  }
}
