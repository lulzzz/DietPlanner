using System;
using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA.DayImplementation;
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

    public NsgaSolver GetDailyDietsSolver(List<Recipe> recipes, DietRequirements dietRequirements)
    {
      return new NsgaSolver(
        new Sorter(),
        new DayPopulationInitializer(_random, recipes, 5, new DietAnalyzer(), dietRequirements),
        new DayEvaluator(new DietAnalyzer(), dietRequirements),
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new DayCrossOver(_random),
        new DayMutator(_random, recipes),
        _configurationProvider.GetConfiguration());
    }
  }
}
