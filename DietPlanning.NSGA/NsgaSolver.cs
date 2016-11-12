﻿using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class NsgaSolver
  {
    //config
    private const double OffspringRatio = 0.3;
    private const double MutationProbability = 0.01;
    private const int PopulationSize = 200;

    private readonly Sorter _sorter;
    private readonly PopulationInitializer _populationInitialiser;
    private readonly Evaluator _evaluator;
    private readonly TournamentSelector _selector;
    private readonly DayCrossOver _crossOver;
    private readonly Mutator _mutator;

    public NsgaSolver(Sorter sorter, PopulationInitializer populationInitialiser, Evaluator evaluator, TournamentSelector selector, DayCrossOver crossOver, Mutator mutator)
    {
      _sorter = sorter;
      _populationInitialiser = populationInitialiser;
      _evaluator = evaluator;
      _selector = selector;
      _crossOver = crossOver;
      _mutator = mutator;
    }

    public List<List<Individual>> Solve(DietSummary targetDietSummary)
    {
      var individuals = InitializeIndividuals();
      var offspringSize = individuals.Count*OffspringRatio;

      _evaluator.Evaluate(individuals, targetDietSummary);
      var fronts = _sorter.Sort(individuals).ToList();
      AssignCrowdingDistance(fronts);

      //Create Offspring
      while(individuals.Count < PopulationSize + offspringSize)
      {
        var children = _crossOver.CreateChildren(_selector.Select(individuals), _selector.Select(individuals));
        
        _mutator.Mutate(children.Item1, MutationProbability);
        _mutator.Mutate(children.Item2, MutationProbability);
        _evaluator.Evaluate(children.Item1, targetDietSummary);
        _evaluator.Evaluate(children.Item2, targetDietSummary);
        individuals.Add(children.Item1);
        individuals.Add(children.Item2);
      }
      //sort
      fronts = _sorter.Sort(individuals);
      AssignCrowdingDistance(fronts);
      //select next Gen
      fronts = SelectNextGeneration(fronts);
      individuals = fronts.SelectMany(f => f).ToList();

      if(individuals.Count != PopulationSize)
        throw new ApplicationException($"Population size is {individuals.Count} instead of {PopulationSize}");

      //return fronts.Select(front => front.Select(individual => individual.Diet).ToList()).ToList();
      return fronts;
    }

    private List<List<Individual>> SelectNextGeneration(List<List<Individual>> fronts)
    {
      var nextGeneration = new List<List<Individual>>();
      var places = PopulationSize;

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
      var diets = _populationInitialiser.InitializePopulation(PopulationSize, 7, 5);
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
