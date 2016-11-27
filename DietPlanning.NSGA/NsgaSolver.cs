﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tools;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    private readonly Configuration _config;
    private readonly Sorter _sorter;
    private readonly IPopulationInitializer _populationInitialiser;
    private readonly IEvaluator _evaluator;
    private readonly TournamentSelector _selector;
    private readonly ICrossOver _crossOver;
    private readonly IMutator _mutator;

    public NsgaSolver(
      Sorter sorter,
      IPopulationInitializer populationInitialiser,
      IEvaluator evaluator, 
      TournamentSelector selector,
      ICrossOver crossOver,
      IMutator mutator, 
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

    public NsgaResult Solve()
    {
      var log = new NsgaLog();

      var individuals = InitializeIndividuals();
      _evaluator.Evaluate(individuals);
      var fronts = _sorter.Sort(individuals).ToList();

      for (var iteration = 0; iteration < _config.MaxIterations; iteration++)
      {
        AssignCrowdingDistance(fronts);
        //Create Offspring
        individuals.AddRange(CreateOffspring(individuals));
        //sort
        fronts = _sorter.Sort(individuals);
        AssignCrowdingDistance(fronts);
        //select next Gen
        fronts = SelectNextGeneration(fronts);
        individuals = fronts.SelectMany(f => f).ToList();

        LogData(log, fronts, iteration);
        Debug.WriteLine($"iteration {iteration}");

        if (individuals.Count != _config.PopulationSize)
          throw new ApplicationException($"Population size is {individuals.Count} instead of {_config.PopulationSize}");
      }

      return new NsgaResult
      {
        Log = log,
        Fronts = fronts
      };
    }

    private void LogData(NsgaLog log, List<List<Individual>> fronts, int iteration)
    {
      log.FrontsNumberLog.Add(fronts.Count);

      var averageMacro = GetFrontAverageEvaluation(fronts.First(), ObjectiveType.Macro);
      var averageCost = GetFrontAverageEvaluation(fronts.First(), ObjectiveType.Cost);
      var averagePrepTime = GetFrontAverageEvaluation(fronts.First(), ObjectiveType.PreparationTime);

      log.ObjectiveLogs.Add( new ObjectiveLog {ObjectiveType = ObjectiveType.Macro, Iteration = iteration, ObjectiveValue = averageMacro });
      log.ObjectiveLogs.Add( new ObjectiveLog {ObjectiveType = ObjectiveType.Cost, Iteration = iteration, ObjectiveValue = averageCost });
      log.ObjectiveLogs.Add( new ObjectiveLog {ObjectiveType = ObjectiveType.PreparationTime, Iteration = iteration, ObjectiveValue = averagePrepTime});

      //CsvLogger.AddRow("iterationEvaluations", new dynamic[]{ iteration, averageMacro, averageCost, fronts.Count});
    }

    private static double GetFrontAverageEvaluation(List<Individual> front, ObjectiveType objectiveType)
    {
      return front
               .Select(diet => diet.Evaluations.Single(evaluation => evaluation.Type == objectiveType).Score)
               .Sum()/ front.Count;
    }

    private List<Individual> CreateOffspring(List<Individual> individuals)
    {
      var offspring = new List<Individual>();
      var offspringSize = _config.PopulationSize * _config.OffspringRatio;

      while (offspring.Count < offspringSize)
      {
        var children = _crossOver.CreateChildren(_selector.Select(individuals), _selector.Select(individuals));

        _mutator.Mutate(children.Item1, _config.MutationProbability);
        _mutator.Mutate(children.Item2, _config.MutationProbability);
        _evaluator.Evaluate(children.Item1);
        _evaluator.Evaluate(children.Item2);
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
      var individuals = _populationInitialiser.InitializePopulation(_config.PopulationSize);

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
