using System;
using System.Collections.Generic;

namespace DietPlanning.NSGA.MathImplementation
{
  public class MathInitializer : IPopulationInitializer
  {
    private readonly Random _random;

    public MathInitializer(Random random)
    {
      _random = random;
    }

    public List<Individual> InitializePopulation(int populationSize)
    {
      var population = new List<Individual>();

      for (var i = 0; i < populationSize; i++)
      {
        population.Add(new MathIndividual
        {
          X1 = (_random.NextDouble() + 0.5)*2.0,
        });
      }

      return population;
    }
  }
}
