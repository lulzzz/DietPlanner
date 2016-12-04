﻿using System;
using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.NSGA.GroupDietsImplementation;
using DietPlanning.NSGA.MathImplementation;

namespace DietPlanning.NSGA
{
  public class NsgaSolverFactory
  {
    private const int TournamentSize = 2;

    private readonly Random _random;

    public NsgaSolverFactory(Random random)
    {
      _random = random;
    }
    
    public NsgaSolver GetMathSolver(Configuration configuration)
    {
      return new NsgaSolver(
        new Sorter(),
        new MathInitializer(_random),
        new MathEvaluator(), 
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new MathCrossOver(_random), 
        new MathMutator(_random),
        configuration);
    }

    public NsgaSolver GetDailyDietsSolver(Configuration configuration, List<Recipe> recipes, DietRequirements dietRequirements, DietPreferences dietPreferences)
    {
      return new NsgaSolver(
        new Sorter(),
        new DayPopulationInitializer(_random, recipes, 5, new DietAnalyzer(), dietRequirements),
        new DayEvaluator(new DietAnalyzer(), dietRequirements, dietPreferences),
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new DayCrossOver(_random),
        new DayMutator(_random, recipes),
        configuration);
    }

    public NsgaSolver GetGroupDietSolver(List<Recipe> recipes, List<PersonalData> personalData, Configuration configuration)
    {
      var groupDietAnalyzer = new GroupDietAnalyzer();

      return new NsgaSolver(
        new Sorter(),
        new GroupDietPopulationInitializer(_random, recipes, 5, personalData, groupDietAnalyzer), 
        new GroupDietEvaluator(personalData, groupDietAnalyzer), 
        new TournamentSelector(new CrowdedDistanceComparer(), TournamentSize, new Random()),
        new GroupDietCrossOver(_random), 
        new GroupDietMutator(_random, recipes, personalData.Count), 
        configuration);
    }
  }
}
