using System;
using System.Linq;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;

namespace MultiAttributeDecisionMaking
{
  public static class EuclidianHelper
  {
    //public static double Euclidian(GroupDietIndividual individual, double[] referencePoint)
    //{
    //  var summarizedScores = SummarizeScores(individual);
    //  //macro, cost, time, preferences
    //  return Math.Sqrt(
    //    Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score / summarizedScores - referencePoint[0], 2) -
    //    Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score / summarizedScores - referencePoint[1], 2) -
    //    Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score / summarizedScores - referencePoint[2], 2) -
    //    Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score / summarizedScores - referencePoint[3], 2));
    //}

    public static double Euclidian(GroupDietIndividual individual, WeightsModel weights)
    {
      var summarizedScores = SummarizeScores(individual);
      var normalziedWeights = NormalizeWeights(weights);

      //macro, cost, time, preferences
      return Math.Sqrt(
        Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score / summarizedScores - normalziedWeights.MacroWeight, 2) -
        Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score / summarizedScores - normalziedWeights.CostWeight, 2) -
        Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score / summarizedScores - normalziedWeights.PreparationTimeWeight, 2) -
        Math.Pow(individual.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score / summarizedScores - normalziedWeights.PreferncesWeight, 2));
    }

    private static double SummarizeScores(GroupDietIndividual individual)
    {
      return individual.Evaluations.Select(e => e.Score).Sum();
    }

    private static WeightsModel NormalizeWeights(WeightsModel weights)
    {
      var sum = weights.CostWeight + weights.MacroWeight + weights.PreferncesWeight + weights.PreparationTimeWeight;

      return new WeightsModel
      {
        MacroWeight = weights.MacroWeight/sum,
        PreparationTimeWeight = weights.PreparationTimeWeight/sum,
        PreferncesWeight = weights.PreferncesWeight/sum,
        CostWeight = weights.CostWeight/sum,
      };
    }
  }
}
