using System;
using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class PopulationInitializer
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;

    public PopulationInitializer(Random random, List<Recipe> recipes)
    {
      _random = random;
      _recipes = recipes;
    }

    public List<Diet> InitializePopulation(int populationSize, int numberOfDays, int numberOfMealsPerDay)
    {
      var population = new List<Diet>();

      for (var i = 0; i < populationSize; i++)
      {
        population.Add(CreateRandomDiet(numberOfDays, numberOfMealsPerDay));
      }

      return population;
    }

    private Diet CreateRandomDiet(int numberOfDays, int numberOfMealsPerDay)
    {
      var diet = new Diet();

      for (var i = 0; i < numberOfDays; i++)
      {
        diet.DailyDiets.Add(CreateRandomDailyDiet(numberOfMealsPerDay));
      }

      return diet;
    }

    private DailyDiet CreateRandomDailyDiet(int numberOfMeals)
    {
      var dailyDiet = new DailyDiet();

      for (var j = 0; j < numberOfMeals; j++)
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
