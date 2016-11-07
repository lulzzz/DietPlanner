using System;
using System.Linq;

namespace DietPlanning.NSGA
{
  public class Mutator
  {
    private readonly Random _random;

    public Mutator(Random random)
    {
      _random = random;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      throw new ArgumentOutOfRangeException();
      foreach (var dailyDiet in individual.Diet.DailyDiets)
      {
        foreach (var meal in dailyDiet.Meals)
        {
          if (_random.NextDouble() < mutationProbability)
          {
            switch (RandomMutationType())
            {
              case MutationType.Remove:
                break;
              case MutationType.Add:
                break;
              case MutationType.Replace:
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
          }
        }
      }
    }

    private MutationType RandomMutationType()
    {
      var randomNumber = _random.NextDouble();
      if (randomNumber < 0.2) return MutationType.Remove;
      if (randomNumber < 0.4) return MutationType.Add;
      return MutationType.Replace;
    }
  }

  internal enum MutationType
  {
    Remove,
    Add,
    Replace
  }
}
