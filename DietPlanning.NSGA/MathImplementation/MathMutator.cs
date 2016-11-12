using System;

namespace DietPlanning.NSGA.MathImplementation
{
  public class MathMutator : IMutator
  {
    private readonly Random _random;

    public MathMutator(Random random)
    {
      _random = random;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      var mathIndividual = individual as MathIndividual;
      if (_random.NextDouble() > mutationProbability)
      {
        var mutationSign = _random.NextDouble() > 0.5 ? 1 : -1;

        mathIndividual.X1 += _random.NextDouble()*0.1*mathIndividual.X1*mutationSign;
      }
      if (_random.NextDouble() > mutationProbability)
      {
        var mutationSign = _random.NextDouble() > 0.5 ? 1 : -1;

        mathIndividual.X2 += _random.NextDouble() * 0.1 * mathIndividual.X2 * mutationSign;
      }
    }
  }
}
