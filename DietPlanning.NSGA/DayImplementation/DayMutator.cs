using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using Tools;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayMutator : IMutator
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;

    public DayMutator(Random random, List<Recipe> recipes)
    {
      _random = random;
      _recipes = recipes;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      //todo may affect performance - consider foreach diet, foreach meal etc
      var meals = ((DayIndividual)individual).DailyDiet.Meals.Select(m => m);

      foreach (var meal in meals)
      {
        for (var i = meal.Receipes.Count - 1; i >= 0; i--)
        {
          if (_random.NextDouble() > mutationProbability) continue;

          PerformMutation(meal, meal.Receipes[i]);
        }
      }
    }

    private void PerformMutation(Meal meal, Recipe recipe)
    {
      const int maxRecipes = 4;
      const int minRecipes = 1;

      switch (RandomMutationType())
      {
        case DietImplementation.MutationType.Remove:
          meal.Receipes.Remove(recipe);
          if (meal.Receipes.Count < minRecipes)
            meal.Receipes.Add(_recipes.GetRandomItem());
          break;
        case DietImplementation.MutationType.Add:
          //todo remove duplicates (or should they stay as 2x recipe)
          meal.Receipes.Add(_recipes.GetRandomItem());
          if (meal.Receipes.Count > maxRecipes)
            meal.Receipes.Remove(recipe);
          break;
        case DietImplementation.MutationType.Replace:
          meal.Receipes.Remove(recipe);
          meal.Receipes.Add(_recipes.GetRandomItem());
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private DietImplementation.MutationType RandomMutationType()
    {
      var randomNumber = _random.NextDouble();
      if (randomNumber < 0.3) return DietImplementation.MutationType.Remove;
      if (randomNumber < 0.6) return DietImplementation.MutationType.Add;
      return DietImplementation.MutationType.Replace;
    }
  }

  internal enum MutationType
  {
    Remove,
    Add,
    Replace
  }
}
