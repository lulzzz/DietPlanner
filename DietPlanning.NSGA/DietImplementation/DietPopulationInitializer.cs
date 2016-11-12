using System;
using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DietImplementation
{
  public class DietPopulationInitializer : IPopulationInitializer
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;
    private readonly int _numberOfDays;
    private readonly int _numberOfMealsPerDay;

    public DietPopulationInitializer(Random random, List<Recipe> recipes, int numberOfDays, int numberOfMealsPerDay)
    {
      _random = random;
      _recipes = recipes;
      _numberOfMealsPerDay = numberOfMealsPerDay;
      _numberOfDays = numberOfDays;
    }

    public List<Individual> InitializePopulation(int populationSize)
    {
      var population = new List<Individual>();

      for (var i = 0; i < populationSize; i++)
      {
        population.Add(new DietIndividual(CreateRandomDiet()));
      }

      return population;
    }

    private Diet CreateRandomDiet()
    {
      var diet = new Diet();

      for (var i = 0; i < _numberOfDays; i++)
      {
        diet.DailyDiets.Add(CreateRandomDailyDiet());
      }

      return diet;
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
      var numberOfRecipes = _random.Next(1, 1);

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
