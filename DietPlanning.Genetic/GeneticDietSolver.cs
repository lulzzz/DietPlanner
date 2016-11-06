using System;
using System.Collections.Generic;
using System.Linq;

using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Genetic.AHP;
using DietPlanning.Genetic.CrossingOver;
using DietPlanning.Genetic.Selectors;

namespace DietPlanning.Genetic
{
  public class GeneticDietSolver
  {
    private readonly Random _random;
    private readonly PopulationInitializer _populationInitializer;
    private readonly ISelector _selector;
    private readonly DaysCrossOver _crossOver;
    private readonly Evaluator _evaluator;

    public GeneticDietSolver(PopulationInitializer populationInitializer, ISelector selector, DaysCrossOver crossOver, Evaluator evaluator)
    {
      _populationInitializer = populationInitializer;
      _selector = selector;
      _crossOver = crossOver;
      _evaluator = evaluator;
      _random = new Random();
    }

    public List<Diet> GetDiet(List<Food> foods, DietSummary targetDiet, Configuration configuration)
    {
      var currentPopulation = _populationInitializer.InitializePopulation(foods, configuration);
      var numberOfIndividualsToSelect = (int)(configuration.PopulationSize* configuration.SelectionFactor);
      var iteration = 0;

      while (iteration < configuration.MaxIterations)
      {
        var ahpSolver = new AhpSolver();
        var ahpEvaluations = ahpSolver.Evaluate(currentPopulation, targetDiet);

        var evaluations = Evaluate(currentPopulation, targetDiet);
        OutputEvaluation(evaluations, iteration);
        
        currentPopulation = _selector.Select(evaluations, numberOfIndividualsToSelect);
        CrossOver(configuration, currentPopulation, numberOfIndividualsToSelect);
        Mutate(currentPopulation, foods, configuration);

        iteration++;
      }

      var endEvaluations = Evaluate(currentPopulation, targetDiet);
      return currentPopulation.OrderBy(diet => _evaluator.Evaluate(diet, targetDiet)).ToList();
    }

    private static void OutputEvaluation(List<KeyValuePair<Diet, double>> evaluations, int iteration)
    {
      var avgEvaluation = evaluations.Select(evaluation => evaluation.Value).Sum()/evaluations.Count;

      if (iteration%100 == 0)
      {
        Console.WriteLine(iteration + " " + avgEvaluation);
      }
    }

    private void CrossOver(Configuration configuration, List<Diet> currentPopulation, int numberOfIndividualsToSelect)
    {
      currentPopulation.AddRange(_crossOver.CreateChildren(currentPopulation, configuration.PopulationSize - numberOfIndividualsToSelect));
    }

    private void Mutate(List<Diet> population, List<Food> foods, Configuration configuration)
    {
      foreach (var individual in population)
      {
        if (_random.NextDouble() <= configuration.MutationProbability)
        {
          Mutate(individual, foods, configuration.NumberOfMealsPerDay, configuration.NumberOfDays);
        }
      }
    }

    private void Mutate(Diet individual, List<Food> foods, int numberOfMeals, int numberOfDays)
    {
      var portionsToMutate = 
        individual
        .DailyDiets[_random.Next(numberOfDays)]
        .Meals[_random.Next(numberOfMeals)]
        .FoodPortions;
      var portionToMutate = portionsToMutate[_random.Next(portionsToMutate.Count)];

      portionToMutate.Food = foods[_random.Next(foods.Count)];
      portionToMutate.Amount += (int)(100 * (_random.NextDouble() - 0.5));

      if (portionToMutate.Amount < 0)
      {
        portionToMutate.Amount = 0;
      }
    }

    private List<KeyValuePair<Diet, double>> Evaluate(List<Diet> population, DietSummary targetDailyDiet)
    {
      return
        population
        .Select(individual => new KeyValuePair<Diet, double>(individual, _evaluator.Evaluate(individual, targetDailyDiet)))
        .ToList();
    }
  }
}
