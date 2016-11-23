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

          PerformMutation(meal, meal.Receipes[i], (DayIndividual)individual);
        }
      }
    }

    private void PerformMutation(Meal meal, Recipe recipe, DayIndividual individual)
    {
      const int maxRecipes = 4;
      const int minRecipes = 1;

      var includedRecipes = individual.DailyDiet.Meals.SelectMany(m => m.Receipes).ToList();

      switch (RandomMutationType())
      {
          

        case MutationType.Remove:
          meal.Receipes.Remove(recipe);
          if (meal.Receipes.Count < minRecipes)
            meal.Receipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
          break;
        case MutationType.Add:
          meal.Receipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
          if (meal.Receipes.Count > maxRecipes)
            meal.Receipes.Remove(recipe);
          break;
        case MutationType.Replace:
          meal.Receipes.Remove(recipe);
          meal.Receipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private MutationType RandomMutationType()
    {
      var randomNumber = _random.NextDouble();
      if (randomNumber < 0.3) return MutationType.Remove;
      if (randomNumber < 0.6) return MutationType.Add;
      return MutationType.Replace;
    }

    private Recipe GetRanodmNotInvlolvedRecipe(List<Recipe> includedRecipes)
    {
      Recipe recipe;

      do
      {
        recipe = _recipes.GetRandomItem();
      } while (includedRecipes.Contains(recipe));

      return recipe;
    }
  }

  internal enum MutationType
  {
    Remove,
    Add,
    Replace
  }
}
