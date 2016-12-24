using System.Linq;
using ConsoleInterface.Storage;
using DietPlanning.NSGA;
using Storage;

namespace Aggregator
{
  public static class Mapper
  {
    public static Individual ToIndividual(ResultPoint resultPoint)
    {
      var individual = new Individual();

      individual.Evaluations.Add(new Evaluation { Type = ObjectiveType.Cost, Score = resultPoint.Cost, Direction = Direction.Minimize });
      individual.Evaluations.Add(new Evaluation { Type = ObjectiveType.Macro, Score = resultPoint.Macro, Direction = Direction.Minimize });
      individual.Evaluations.Add(new Evaluation { Type = ObjectiveType.Preferences, Score = resultPoint.Preferences, Direction = Direction.Minimize });
      individual.Evaluations.Add(new Evaluation { Type = ObjectiveType.PreparationTime, Score = resultPoint.PreparationTime, Direction = Direction.Minimize });

      return individual;
    }

    public static ResultPoint CreateResultPoint(Individual individual)
    {
      return new ResultPoint
      {
        Cost = individual.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score,
        Preferences = individual.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score,
        PreparationTime = individual.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score,
        Macro = individual.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score
      };
    }
  }
}
