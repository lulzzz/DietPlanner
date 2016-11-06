using System;
using System.Collections.Generic;
using System.Linq;

using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Genetic.Selectors
{
  public class RuletteWheelSelector : ISelector
  {
    private readonly Random _random;

    public RuletteWheelSelector()
    {
      _random = new Random();
    }

    public List<Diet> Select(List<KeyValuePair<Diet, double>> evaluatedPopulation, int numberOfIndividualsToSelect)
    {
      var totalFitness = evaluatedPopulation.Select(individual => individual.Value).Sum();
      var selectedIndividuals = new List<Diet>();
      var currentFitness = 0.0;
      var boundaries = SetProbabilityBoundaries(evaluatedPopulation, currentFitness, totalFitness);

      while (numberOfIndividualsToSelect > selectedIndividuals.Count)
      {
        selectedIndividuals.Add(SelectIndividual(evaluatedPopulation, boundaries));
      }

      return selectedIndividuals;
    }

    private Diet SelectIndividual(List<KeyValuePair<Diet, double>> evaluatedPopulation, List<Tuple<double, double>> boundaries)
    {
      var random = _random.NextDouble();
      var selectedBoundary = boundaries.First(boundary => random < boundary.Item2);
      var selectedIndividual = evaluatedPopulation[boundaries.IndexOf(selectedBoundary)].Key;

      return selectedIndividual;
    }

    private static List<Tuple<double, double>> SetProbabilityBoundaries(List<KeyValuePair<Diet, double>> evaluatedPopulation, double currentFitness, double totalFitness)
    {
      var boundaries = new List<Tuple<double, double>>(evaluatedPopulation.Capacity);

      foreach (var individual in evaluatedPopulation)
      {
        boundaries.Add(new Tuple<double, double>(currentFitness, currentFitness += individual.Value/totalFitness));
      }

      return boundaries;
    }
  }
}
