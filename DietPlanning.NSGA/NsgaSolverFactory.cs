using System;
using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.NSGA.DietImplementation;
using DietPlanning.NSGA.MathImplementation;

namespace DietPlanning.NSGA
{
  public class NsgaSolverFactory
  {
    private const int TournamentSize = 2;

    private readonly ConfigurationProvider _configurationProvider;
    private readonly Random _random;

    public NsgaSolverFactory(ConfigurationProvider configurationProvider, Random random)
    {
      _configurationProvider = configurationProvider;
      _random = random;
    }

    public NsgaSolver GetDietSolver(List<Recipe> recipes, DietSummary targetDiet)
    {
      return new NsgaSolver(
        new Sorter(),
        new DietPopulationInitializer(_random, recipes, 7, 5),
        new DietEvaluator(new DietAnalyzer(), targetDiet),
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new DayCrossOver(_random),
        new DietMutator(_random, recipes),
        _configurationProvider.GetConfiguration());
    }

    public NsgaSolver GetMathSolver()
    {
      return new NsgaSolver(
        new Sorter(),
        new MathInitializer(_random),
        new MathEvaluator(), 
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new MathCrossOver(_random), 
        new MathMutator(_random), 
        _configurationProvider.GetConfiguration());
    }
  }
}
