using System;
using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;
using Tools;

namespace DietPlanning.NSGA.DayImplementation
{
  class DayPopulationInitializer : IPopulationInitializer
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;
    private readonly int _numberOfMealsPerDay;
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietRequirements _requirements;

    public DayPopulationInitializer(Random random, List<Recipe> recipes, int numberOfMealsPerDay, DietAnalyzer dietAnalyzer, DietRequirements requirements)
    {
      _random = random;
      _recipes = recipes;
      _numberOfMealsPerDay = numberOfMealsPerDay;
      _dietAnalyzer = dietAnalyzer;
      _requirements = requirements;
    }

    public List<Individual> InitializePopulation(int populationSize)
    {
      var population = new List<Individual>();

      for (var i = 0; i < populationSize; i++)
      {
        population.Add(new DayIndividual(CreateRandomDailyDiet()));
      }

      return population;
    }

    private DailyDiet CreateRandomDailyDiet()
    {
      var dailyDiet = new DailyDiet();

      for (var j = 0; j < _numberOfMealsPerDay; j++)
      {
        var meal = CreateRandomMeal();
        meal.MealType = (MealType)j;
        dailyDiet.Meals.Add(meal);
      }

      while (_requirements.CaloriesAllowedRange.IsInRange(_dietAnalyzer.SummarizeDaily(dailyDiet).NutritionValues.Calories))
      {
        AddRandomReceipe(dailyDiet.Meals.GetRandomItem());
      }

      return dailyDiet;
    }

    private Meal CreateRandomMeal()
    {
      var meal = new Meal();
      var numberOfRecipes = _random.Next(1, 4);

      for (var k = 0; k < numberOfRecipes; k++)
      {
        AddRandomReceipe(meal);
      }

      return meal;
    }

    private void AddRandomReceipe(Meal meal)
    {
      Recipe recipe;

      do
      {
        recipe = _recipes.GetRandomItem();
      } while (meal.Receipes.Contains(recipe));

      meal.Receipes.Add(recipe);
    }
  }
}
