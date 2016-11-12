using System;

namespace DietPlanning.NSGA
{
  public interface ICrossOver
  {
    Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2);
  }
}
