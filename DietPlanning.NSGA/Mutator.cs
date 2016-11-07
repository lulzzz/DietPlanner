using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using Tools;

namespace DietPlanning.NSGA
{
  public class Mutator
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;

    public Mutator(Random random, List<Recipe> recipes)
    {
      _random = random;
      _recipes = recipes;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      //todo may affect performance - consider foreach diet, foreach meal etc
      var meals = individual.Diet.DailyDiets.SelectMany(dailyDiet => dailyDiet.Meals);

      foreach (var meal in meals)
      {
        foreach (var recipe in meal.Receipes)
        {
          if (_random.NextDouble() > mutationProbability) continue;

          PerformMutation(meal, recipe);
        }
      }
    }

    private void PerformMutation(Meal meal, Recipe recipe)
    {
      switch (RandomMutationType())
      {
        case MutationType.Remove:
          meal.Receipes.Remove(recipe);
          break;
        case MutationType.Add:
          //todo remove duplicates (or should they stay as 2x recipe)
          meal.Receipes.Add(_recipes.GetRandomItem());
          break;
        case MutationType.Replace:
          meal.Receipes.Remove(recipe);
          meal.Receipes.Add(_recipes.GetRandomItem());
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private MutationType RandomMutationType()
    {
      var randomNumber = _random.NextDouble();
      if (randomNumber < 0.2) return MutationType.Remove;
      if (randomNumber < 0.4) return MutationType.Add;
      return MutationType.Replace;
    }
  }

  internal enum MutationType
  {
    Remove,
    Add,
    Replace
  }
}
