using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    private readonly Sorter _sorter;

    public NsgaSolver(Sorter sorter)
    {
      _sorter = sorter;
    }

    public List<List<Diet>> Solve(List<Diet> diets)
    {
      var individuals = diets.Select(i => new Individual(i)).ToList();
      var fronts = _sorter.Sort(individuals);

      return fronts.Select(front => front.Select(individual => individual.Diet).ToList()).ToList();
    }
  }
}
