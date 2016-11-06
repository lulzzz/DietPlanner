﻿using System.Collections.Generic;
using System.Linq;

namespace DietPlanning.NSGA
{
  public class Sorter
  {
    public List<List<Individual>> Sort(List<Individual> individuals)
    {
      var fronts = new List<List<Individual>> { SetDomintaionRelations(individuals) };

      CreateSubsequentFronts(fronts, individuals);

      return fronts;
    }

    private static void CreateSubsequentFronts(List<List<Individual>> fronts, List<Individual> individuals)
    {
      var currentFront = fronts.Last();
      var rank = 0;

      do
      {
        //TODO: Sort by numbers od dominating solutions (no need to loop)
        //TODO: Instead of decreasing each count store number of front and compare
        var nextFront = new List<Individual>();
        
        foreach (var p in currentFront)
        {
          foreach (var q in p.Dominated)
          {
            q.DominatedByCount--;
            if (q.DominatedByCount == 0)
            {
              q.Rank = rank;
              nextFront.Add(q);
              break;
            }
          }
        }
        if (nextFront.Count > 0)
        {
          fronts.Add(nextFront);
          rank++;
        }
      } while (individuals.Count > fronts.Select(f => f.Count).Sum());
    }

    private List<Individual> SetDomintaionRelations(List<Individual> individuals)
    {
      var front = new List<Individual>();

      foreach (var p1 in individuals)
      {
        foreach (var p2 in individuals)
        {
          if (Dominates(p1, p2))
            p1.Dominated.Add(p2);
          else if (Dominates(p2, p1))
            p1.DominatedByCount++;
        }
        if (p1.DominatedByCount == 0)
          front.Add(p1);
      }

      return front;
    }

    public bool Dominates(Individual individual1, Individual individual2)
    {
      return !individual1
         .Evaluations
        .Where((evaluation, i) => evaluation.Score < individual2.Evaluations[i].Score)
        .Any();
    }
  }
}

