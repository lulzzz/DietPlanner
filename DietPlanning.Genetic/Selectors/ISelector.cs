using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Genetic.Selectors
{
  public interface ISelector
  {
    List<Diet> Select(List<KeyValuePair<Diet, double>> evaluatedPopulation, int numberOfIndividualsToSelect);
  }
}
