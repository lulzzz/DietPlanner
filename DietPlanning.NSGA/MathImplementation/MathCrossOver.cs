using System;

namespace DietPlanning.NSGA.MathImplementation
{
  public class MathCrossOver : ICrossOver
  {
    public Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2)
    {
      var mathParent1 = parent1 as MathIndividual;
      var mathParent2 = parent1 as MathIndividual;

      var child1 = new MathIndividual {X1 = mathParent1.X1, X2 = mathParent2.X2};
      var child2 = new MathIndividual {X1 = mathParent2.X1, X2 = mathParent1.X2};

      return new Tuple<Individual, Individual>(child1, child2);
    }
  }
}
