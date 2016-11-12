using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DietPlanning.Core;
using Tools;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    private readonly Configuration _config;
    private readonly Sorter _sorter;
    private readonly PopulationInitializer _populationInitialiser;
    private readonly Evaluator _evaluator;
    private readonly TournamentSelector _selector;
    private readonly DayCrossOver _crossOver;
    private readonly Mutator _mutator;

    public NsgaSolver(
      Sorter sorter, 
      PopulationInitializer populationInitialiser,
      Evaluator evaluator, 
      TournamentSelector selector, 
      DayCrossOver crossOver, 
      Mutator mutator, 
      Configuration configuration)
    {
      _sorter = sorter;
      _populationInitialiser = populationInitialiser;
      _evaluator = evaluator;
      _selector = selector;
      _crossOver = crossOver;
      _mutator = mutator;
      _config = configuration;
    }

    public List<List<Individual>> Solve(DietSummary targetDietSummary)
    {
      var individuals = InitializeIndividuals();
      _evaluator.Evaluate(individuals, targetDietSummary);
      var fronts = _sorter.Sort(individuals).ToList();

      for (var iteration = 0; iteration < _config.MaxIterations; iteration++)
      {
        AssignCrowdingDistance(fronts);
        //Create Offspring
        individuals.AddRange(CreateOffspring(targetDietSummary, individuals));
        //sort
        fronts = _sorter.Sort(individuals);
        AssignCrowdingDistance(fronts);
        //select next Gen
        fronts = SelectNextGeneration(fronts);
        individuals = fronts.SelectMany(f => f).ToList();

        LogData(fronts, iteration);
        Debug.WriteLine($"iteration {iteration}");

        if (individuals.Count != _config.PopulationSize)
          throw new ApplicationException($"Population size is {individuals.Count} instead of {_config.PopulationSize}");
      }

      return fronts;
    }

    private void LogData(List<List<Individual>> fronts, int iteration)
    {
      var avgMacroEv = fronts.First()
        .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Macro).Score)
        .Sum()/fronts.First().Count;
      var avgCostEv = fronts.First()
        .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Cost).Score)
        .Sum() / fronts.First().Count;
      //var avgVarEv = fronts.First()
      //  .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Variety).Score)
      //  .Sum() / fronts.First().Count;
      //var avgPrepEv = fronts.First()
      //  .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.PreparationTime).Score)
      //  .Sum() / fronts.First().Count;
      var avgMacroEv2 = fronts[1]
        .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Macro).Score)
        .Sum() / fronts[1].Count;
      var avgCostEv2 = fronts[1]
        .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Cost).Score)
        .Sum() / fronts[1].Count;
      //var avgVarEv2 = fronts[1]
      //  .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.Variety).Score)
      //  .Sum() / fronts[1].Count;
      //var avgPrepEv2 = fronts[1]
      //  .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == ObjectiveType.PreparationTime).Score)
      //  .Sum() / fronts[1].Count;

      CsvLogger.AddRow(new dynamic[]{ iteration, avgMacroEv, avgCostEv, avgMacroEv2, avgCostEv2 });
    }

    private List<Individual> CreateOffspring(DietSummary targetDietSummary, List<Individual> individuals)
    {
      var offspring = new List<Individual>();
      var offspringSize = _config.PopulationSize * _config.OffspringRatio;

      while (offspring.Count < offspringSize)
      {
        var children = _crossOver.CreateChildren(_selector.Select(individuals), _selector.Select(individuals));

        _mutator.Mutate(children.Item1, _config.MutationProbability);
        _mutator.Mutate(children.Item2, _config.MutationProbability);
        _evaluator.Evaluate(children.Item1, targetDietSummary);
        _evaluator.Evaluate(children.Item2, targetDietSummary);
        offspring.Add(children.Item1);
        offspring.Add(children.Item2);
      }

      return offspring;
    }

    private List<List<Individual>> SelectNextGeneration(List<List<Individual>> fronts)
    {
      var nextGeneration = new List<List<Individual>>();
      var places = _config.PopulationSize;

      foreach (var front in fronts)
      {
        if (front.Count < places)
        {
          nextGeneration.Add(front);
          places -= front.Count;
        }
        else
        {
          var lastFront = front.OrderByDescending(individual => individual.CrowdingDistance).Take(places).ToList();
          nextGeneration.Add(lastFront);
          break;
        }
      }

      return nextGeneration;
    }

    private List<Individual> InitializeIndividuals()
    {
      var diets = _populationInitialiser.InitializePopulation(_config.PopulationSize, _config.NumberOfDays, _config.NumberOfMealsPerDay);
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
