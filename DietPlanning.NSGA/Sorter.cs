using System.Collections.Generic;
using System.Linq;

namespace DietPlanning.NSGA
{
  public class Sorter
  {
    public List<List<Individual>> Sort(List<Individual> individuals)
    {
      foreach (var individual in individuals)
      {
        individual.Dominated.Clear();
        individual.DominatedByCount = 0;
        individual.Rank = 0;
      }

      var fronts = new List<List<Individual>> { SetDomintaionRelations(individuals) };

      CreateSubsequentFronts(fronts, individuals);

      return fronts;
    }

    private static void CreateSubsequentFronts(List<List<Individual>> fronts, List<Individual> individuals)
    {
      var currentFront = fronts.Last();
      var rank = 2;

      do
      {
        //TODO: Sort by numbers od dominating solutions (no need to loop)
        //TODO: Instead of decreasing each count store number of front and compare
        var nextFront = new List<Individual>();
        
        foreach (var individual in currentFront)
        {
          foreach (var dominated in individual.Dominated)
          {
            dominated.DominatedByCount--;
            if (dominated.DominatedByCount == 0)
            {
              dominated.Rank = rank;
              nextFront.Add(dominated);
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
        {
          p1.Rank = 1;
          front.Add(p1);
        }
      }

      return front;
    }

    private bool Dominates(Individual individual1, Individual individual2)
    {
      return !individual1
         .Evaluations
        .Where((evaluation, i) => evaluation.Score < individual2.Evaluations[i].Score)
        .Any()
        
        && individual1
         .Evaluations
        .Where((evaluation, i) => evaluation.Score > individual2.Evaluations[i].Score)
        .Any();
    }
  }
}

