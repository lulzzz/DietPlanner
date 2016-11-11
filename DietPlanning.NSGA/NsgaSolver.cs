using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    //config
    private const double OffspringRatio = 0.5;

    private readonly Sorter _sorter;
    private readonly PopulationInitializer _populationInitialiser;
    private readonly Evaluator _evaluator;
    private readonly TournamentSelector _selector;
    private readonly DayCrossOver _crossOver;

    public NsgaSolver(Sorter sorter, PopulationInitializer populationInitialiser, Evaluator evaluator, TournamentSelector selector, DayCrossOver crossOver)
    {
      _sorter = sorter;
      _populationInitialiser = populationInitialiser;
      _evaluator = evaluator;
      _selector = selector;
      _crossOver = crossOver;
    }

    public List<List<Diet>> Solve(DietSummary targetDietSummary)
    {
      var individuals = InitializeIndividuals();
      var offspringSize = individuals.Count*OffspringRatio;

      _evaluator.Evaluate(individuals, targetDietSummary);
      var fronts = _sorter.Sort(individuals).ToList();
      AssignCrowdingDistance(fronts);

      //Create Offspring
      //for (var i = 0; i < offspringSize; i++)
      //{
      //  var children = _crossOver.CreateChildren(_selector.Select(individuals), _selector.Select(individuals));
      //  individuals.Add(children.Item1);
      //  individuals.Add(children.Item2);

      //  //todo MUTATION
      //}

      return fronts.Select(front => front.Select(individual => individual.Diet).ToList()).ToList();
    }

    private List<Individual> InitializeIndividuals()
    {
      var diets = _populationInitialiser.InitializePopulation(100, 7, 5);
      var individuals = diets.Select(i => new Individual(i)).ToList();
      return individuals;
    }

    private void AssignCrowdingDistance(List<List<Individual>> fronts)
    {
      var numberOfObjectives = fronts.First().First().Evaluations.Select(evaluation => evaluation.Type).Count();

      foreach (var front in fronts)
      {
        front.ForEach(individual => individual.CrowdingDistance = 0);

        for (var evaluationIndex = 0; evaluationIndex < numberOfObjectives; evaluationIndex++)
        {
          var individualsByObjective = front.OrderBy(individual => individual.Evaluations[evaluationIndex].Score).ToList();

          individualsByObjective.First().CrowdingDistance = double.PositiveInfinity;
          individualsByObjective.Last().CrowdingDistance = double.PositiveInfinity;

          var minEvaluation = individualsByObjective.Last().Evaluations[evaluationIndex].Score;
          var maxEvaluation = individualsByObjective.First().Evaluations[evaluationIndex].Score;
          var evaluationRange = maxEvaluation - minEvaluation;

          for (var individualIndex = 1; individualIndex < front.Count - 1; individualIndex++)
          {
            individualsByObjective[individualIndex].CrowdingDistance +=
              Math.Abs(
                (individualsByObjective[individualIndex + 1].Evaluations[evaluationIndex].Score -
                 individualsByObjective[individualIndex - 1].Evaluations[evaluationIndex].Score) /
                evaluationRange);
          }
        }
      }
    }
  }
}
