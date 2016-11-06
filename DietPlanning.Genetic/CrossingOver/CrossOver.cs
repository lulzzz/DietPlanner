using System;
using System.Collections.Generic;
using System.Linq;

using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Genetic.CrossingOver
{
  public class DaysCrossOver
  {
    private readonly Random _random;

    public DaysCrossOver(Random random)
    {
      _random = random;
    }

    public List<Diet> CreateChildren(List<Diet> parentDiets, int numberOfChildren)
    {
      var children = new List<Diet>();

      for (var i = 0; i < numberOfChildren; i++)
      {
        children.Add(GetChild(SelectPairOfParents(parentDiets)));
      }

      return children;
    }

    private Tuple<Diet,Diet> SelectPairOfParents(List<Diet> parentDiets)
    {
      var firstParentIndex = _random.Next(0, parentDiets.Count);
      var secondParentIndex = 0;

      do
      {
        secondParentIndex = _random.Next(0, parentDiets.Count);
      } while (firstParentIndex == secondParentIndex);

      return new Tuple<Diet, Diet>(parentDiets[firstParentIndex], parentDiets[secondParentIndex]);
    }

    private Diet GetChild(Tuple<Diet, Diet> parents)
    {
      var crossoverDay1 = _random.Next(0, parents.Item1.DailyDiets.Count-1);
      var crossoverDay2 = _random.Next(crossoverDay1+1, parents.Item1.DailyDiets.Count);
     
      var child = new Diet();

      child.DailyDiets.AddRange(parents.Item1.DailyDiets.Take(crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child.DailyDiets.AddRange(parents.Item2.DailyDiets.Skip(crossoverDay1).Take(crossoverDay2-crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child.DailyDiets.AddRange(parents.Item1.DailyDiets.Skip(crossoverDay2).Select(DietCopier.CopyDailyDiet));

      return child;
    }
  }
}
