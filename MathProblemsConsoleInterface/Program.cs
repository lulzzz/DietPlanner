using System;
using System.Linq;
using DietPlanning.NSGA;
using Tools;

namespace MathProblemsConsoleInterface
{
  public class Program
  {
    static void Main(string[] args)
    {
      var nsgaFactory = new NsgaSolverFactory(new Random());
      var configuration = new ConfigurationProvider().GetConfiguration();

      configuration.PopulationSize = 300;
      configuration.MaxIterations = 400;

      var mathSolver = nsgaFactory.GetMathSolver(configuration);

      var result = mathSolver.Solve();

      CsvLogger.RegisterLogger("math");

      foreach (var res in result.Fronts.First().Select(i => i.Evaluations))
      {
        CsvLogger.AddRow("math", new dynamic[] {res[0].Score, res[1].Score});
      }

      CsvLogger.Write("math", "math.csv");

      Console.WriteLine("done");
    }
  }
}
