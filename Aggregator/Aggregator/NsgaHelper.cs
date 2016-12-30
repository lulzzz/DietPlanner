using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DietPlanning.NSGA;
using Storage;

namespace Aggregator
{
  public static class NsgaHelper
  {
    public static List<Individual> FindFirstFront(List<Individual> individuals)
    {
      var front = new List<Individual>();

      foreach (var p1 in individuals)
      {
        if(p1.DominatedByCount > 0) continue;

        foreach (var p2 in individuals)
        {
          if (p2.DominatedByCount > 2) continue;

          if (p1 == p2) continue;
          if (Dominates(p1, p2))
          {
            p2.DominatedByCount++;
          }
          else if (Dominates(p2, p1))
          {
            p1.DominatedByCount++;
            break;
          }
        }
        if (p1.DominatedByCount == 0)
        {
          p1.Rank = 1;
          front.Add(p1);
        }
      }

      return front;
    }

    public static void AssignCrowdingDistances(List<Individual> front)
    {
      var numberOfObjectives = front.First().Evaluations.Count;

      front.ForEach(individual => individual.CrowdingDistance = 0);

      for (var evaluationIndex = 0; evaluationIndex < numberOfObjectives; evaluationIndex++)
      {
        var individualsByObjective = front.OrderBy(individual => individual.Evaluations[evaluationIndex].Score).ToList();

        individualsByObjective.First().CrowdingDistance = double.PositiveInfinity;
        individualsByObjective.Last().CrowdingDistance = double.PositiveInfinity;

        var minEvaluation = individualsByObjective.Last().Evaluations[evaluationIndex].Score;
        var maxEvaluation = individualsByObjective.First().Evaluations[evaluationIndex].Score;
        var evaluationRange = maxEvaluation - minEvaluation;

        if (evaluationRange == 0)
        {
          continue;
        }

        for (var individualIndex = 1; individualIndex < front.Count - 1; individualIndex++)
        {
          individualsByObjective[individualIndex].CrowdingDistance +=
            Math.Abs(
              (individualsByObjective[individualIndex + 1].Evaluations[evaluationIndex].Score -
               individualsByObjective[individualIndex - 1].Evaluations[evaluationIndex].Score) /
              evaluationRange);
        }
      }
    }

    public static void AssignCrowdingDistances(List<ResultPoint> resultPoints)
    {
      resultPoints.ForEach(p => p.CrowdingDistance = 0);

      var scoreGetters = new List<Func<ResultPoint, double>>
      {
        (rp) => rp.Macro,
        (rp) => rp.Cost,
        (rp) => rp.Preferences,
        (rp) => rp.PreparationTime
      };

      foreach (var scoreGetter in scoreGetters)
      {
        var poiontsByScore = resultPoints.OrderBy(scoreGetter).ToList();

        poiontsByScore.First().CrowdingDistance = double.PositiveInfinity;
        poiontsByScore.Last().CrowdingDistance = double.PositiveInfinity;

        var minEvaluation = scoreGetter(poiontsByScore.Last());
        var maxEvaluation = scoreGetter(poiontsByScore.First());
        var evaluationRange = maxEvaluation - minEvaluation;

        if (evaluationRange == 0)
        {
          continue;
        }

        for (var index = 1; index < poiontsByScore.Count - 1; index++)
        {
          poiontsByScore[index].CrowdingDistance +=
            Math.Abs((scoreGetter(poiontsByScore[index + 1])  - scoreGetter(poiontsByScore[index - 1])) / evaluationRange);
        }
      }
    }


    private static bool Dominates(Individual individual1, Individual individual2)
    {
      if (individual1.IsFeasible == individual2.IsFeasible)
      {
        return (!individual1
         .Evaluations
        .Where((evaluation, i) => evaluation < individual2.Evaluations[i])
        .Any())
        &&
        individual1
        .Evaluations
        .Where((evaluation, i) => evaluation > individual2.Evaluations[i])
        .Any();
      }

      return individual1.IsFeasible;
    }
  }
}
