using System;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DietImplementation
{
  public class DietCrossOver : ICrossOver
  {
    private readonly Random _random;

    public DietCrossOver(Random random)
    {
      _random = random;
    }

    public Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2)
    {
      var childrenDiets = GetChild(((DietIndividual)parent1).Diet, ((DietIndividual)parent2).Diet);
      
      return new Tuple<Individual, Individual>(new DietIndividual(childrenDiets.Item1), new DietIndividual(childrenDiets.Item2));
    }

    private Tuple<Diet, Diet> GetChild(Diet parent1, Diet parent2)
    {
      var crossoverDay1 = _random.Next(0, parent1.DailyDiets.Count-1);
      var crossoverDay2 = _random.Next(crossoverDay1+1, parent1.DailyDiets.Count);
     
      var child1 = new Diet();
      var child2 = new Diet();

      child1.DailyDiets.AddRange(parent1.DailyDiets.Take(crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child1.DailyDiets.AddRange(parent2.DailyDiets.Skip(crossoverDay1).Take(crossoverDay2-crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child1.DailyDiets.AddRange(parent1.DailyDiets.Skip(crossoverDay2).Select(DietCopier.CopyDailyDiet));

      child2.DailyDiets.AddRange(parent2.DailyDiets.Take(crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child2.DailyDiets.AddRange(parent1.DailyDiets.Skip(crossoverDay1).Take(crossoverDay2 - crossoverDay1).Select(DietCopier.CopyDailyDiet));
      child2.DailyDiets.AddRange(parent2.DailyDiets.Skip(crossoverDay2).Select(DietCopier.CopyDailyDiet));
      
      return new Tuple<Diet, Diet>(child1, child2);
    }
  }
}
