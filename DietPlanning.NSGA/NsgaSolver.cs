using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    private readonly Sorter _sorter;
    private readonly PopulationInitializer _populationInitialiser;
    private readonly Evaluator _evaluator;

    public NsgaSolver(Sorter sorter, PopulationInitializer populationInitialiser, Evaluator evaluator)
    {
      _sorter = sorter;
      _populationInitialiser = populationInitialiser;
      _evaluator = evaluator;
    }

    public List<List<Diet>> Solve(DietSummary targetDietSummary)
    {
      var diets = _populationInitialiser.InitializePopulation(100, 7, 5);
      var individuals = diets.Select(i => new Individual(i)).ToList();

      _evaluator.Evaluate(individuals, targetDietSummary);

      var fronts = _sorter.Sort(individuals);
      AssignCrowdingDistance(fronts);


      return fronts.Select(front => front.Select(individual => individual.Diet).ToList()).ToList();
    }

    private void AssignCrowdingDistance(List<List<Individual>> fronts)
    {
      var numberOfObjectives = fronts.First().First().Evaluations.Select(evaluation => evaluation.Type).Count();

      foreach (var front in fronts)
      {
        front.ForEach(individual => individual.CrowdingDistance = 0);

        for (var evaluationIndex = 0; evaluationIndex < numberOfObjectives; evaluationIndex++)
        {
          var individualsByObjective = front.OrderBy(individual => individual.Evaluations[evaluationIndex]).ToList();

          individualsByObjective.First().CrowdingDistance = double.PositiveInfinity;
          individualsByObjective.Last().CrowdingDistance = double.PositiveInfinity;

          var minEvaluation = individualsByObjective.Last().Evaluations[evaluationIndex].Score;
          var maxEvaluation = individualsByObjective.First().Evaluations[evaluationIndex].Score;
          var evaluationRange = maxEvaluation - minEvaluation;

          for (var individualIndex = 1; individualIndex < front.Count - 1; individualIndex++)
          {
            individualsByObjective[individualIndex].CrowdingDistance +=
              (individualsByObjective[individualIndex + 1].Evaluations[evaluationIndex].Score -
               individualsByObjective[individualIndex - 1].Evaluations[evaluationIndex].Score)/
              evaluationRange;
          }
        }
      }
    }
  }
}
