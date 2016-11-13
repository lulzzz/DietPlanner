using System;
using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DayImplementation
{
  class DayPopulationInitializer : IPopulationInitializer
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;
    private readonly int _numberOfMealsPerDay;

    public DayPopulationInitializer(Random random, List<Recipe> recipes, int numberOfMealsPerDay)
    {
      _random = random;
      _recipes = recipes;
      _numberOfMealsPerDay = numberOfMealsPerDay;
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
        dailyDiet.Meals.Add(CreateRandomMeal());
      }

      return dailyDiet;
    }

    private Meal CreateRandomMeal()
    {
      var meal = new Meal();
      var numberOfRecipes = _random.Next(1, 4);

      for (var k = 0; k < numberOfRecipes; k++)
      {
        Recipe recipe;

        do
        {
          recipe = _recipes[_random.Next(_recipes.Count)];
        } while (meal.Receipes.Contains(recipe));

        meal.Receipes.Add(recipe);
      }

      return meal;
    }
  }
}
