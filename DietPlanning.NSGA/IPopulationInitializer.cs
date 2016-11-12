using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public interface IPopulationInitializer
  {
    List<Individual> InitializePopulation(int populationSize);
  }
}
