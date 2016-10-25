using System;
using System.Collections.Generic;

using DietPlanning.Core;

namespace DietPlanning.Genetic
{
  public class PopulationInitializer
  {
    private readonly Random _random;

    public PopulationInitializer(Random random)
    {
      _random = random;
    }

    public List<Diet> InitializePopulation(List<Food> foods, Configuration configuration)
    {
      var population = new List<Diet>();

      for (var i = 0; i < configuration.PopulationSize; i++)
      {
        population.Add(CreateRandomDiet(configuration, foods));
      }

      return population;
    }

    private Diet CreateRandomDiet(Configuration configuration, List<Food> foods)
    {
      var diet = new Diet();

      for (var i = 0; i < configuration.NumberOfDays; i++)
      {
        diet.DailyDiets.Add(CreateRandomDailyDiet(configuration.NumberOfMealsPerDay, foods));
      }

      return diet;
    }

    private DailyDiet CreateRandomDailyDiet(int numberOfMeals, List<Food> foods)
    {
      var dailyDiet = new DailyDiet();

      for (var j = 0; j < numberOfMeals; j++)
      {
        dailyDiet.Meals.Add(CreateRandomMeal(foods));
      }

      return dailyDiet;
    }

    private Meal CreateRandomMeal(List<Food> foods)
    {
      var meal = new Meal();
      var numberOfPortions = _random.Next(1, 4);

      for (var k = 0; k < numberOfPortions; k++)
      {
        meal.FoodPortions.Add(CreateFoodPortion(foods));
      }

      return meal;
    }

    private FoodPortion CreateFoodPortion(List<Food> foods)
    {
      var chosenFood = foods[_random.Next(0, foods.Count)];

      return new FoodPortion(chosenFood, _random.Next(100, 200));
    }
  }
}
