using System.Collections.Generic;
using DietPlanning.Core;

namespace DietPlanning.Genetic.Selectors
{
  public interface ISelector
  {
    List<Diet> Select(List<KeyValuePair<Diet, double>> evaluatedPopulation, int numberOfIndividualsToSelect);
  }
}
