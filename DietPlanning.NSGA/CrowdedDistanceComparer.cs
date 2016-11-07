using System;
using System.Collections.Generic;
using System.Linq;
namespace DietPlanning.NSGA
{
  public class CrowdedDistanceComparer : IComparer<Individual>
  {
    public int Compare(Individual x, Individual y)
    {
      if (x.Rank != y.Rank) return x.Rank < y.Rank ? 1 : -1;
      if (x.CrowdingDistance > y.CrowdingDistance) return 1;
      if (x.CrowdingDistance < y.CrowdingDistance) return -1;

      return 0;
    }
  }
}
