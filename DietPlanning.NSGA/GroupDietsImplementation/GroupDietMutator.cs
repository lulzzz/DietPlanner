using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.GroupDiets;
using Tools;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public class GroupDietMutator : IMutator
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;
    private readonly int _groupSize;
    private static readonly List<double> Multipliers = new List<double> { 0.75, 0.85, 1 , 1.25, 1.5};

    public GroupDietMutator(Random random, List<Recipe> recipes, int groupSize)
    {
      _random = random;
      _recipes = recipes;
      _groupSize = groupSize;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      //todo may affect performance - consider foreach diet, foreach meal etc
      var meals = ((GroupDietIndividual)individual).GroupDiet.Meals.Select(m => m);

      foreach (var meal in meals)
      {
        for (var i = meal.Recipes.Count - 1; i >= 0; i--)
        {
          if (_random.NextDouble() < mutationProbability)
          {
            PerformMealLevelMutation(meal, meal.Recipes[i], (GroupDietIndividual)individual);
          }
          else
          {
            PerformRecipesLevelMutation(meal, mutationProbability);
          }
        }
      }
    }

    private void PerformRecipesLevelMutation(GroupMeal meal, double mutationProbability)
    {
      foreach (var recipeGroupSplit in meal.Recipes)
      {
        if (_random.NextDouble() < mutationProbability)
        {
          switch (RandomMutationType())
          {
            case MutationType.Remove:
              if (recipeGroupSplit.Adjustments.Count > 1)
                recipeGroupSplit.Adjustments.Remove(recipeGroupSplit.Adjustments.GetRandomItem());
              break;
            case MutationType.Add:
              if (recipeGroupSplit.Adjustments.Count < _groupSize)
                recipeGroupSplit.Adjustments.Add(GetRandomMissingAdjustment(recipeGroupSplit.Adjustments));
              break;
            case MutationType.Replace:
              recipeGroupSplit.Adjustments.GetRandomItem().AmountMultiplier = Multipliers.GetRandomItem();
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    private RecipeAdjustment GetRandomMissingAdjustment(List<RecipeAdjustment> adjustments)
    {
      int id;

      do
      {
        id = _random.Next(_groupSize);
      } while (adjustments.Any(ad => ad.PersonId == id));

      return new RecipeAdjustment {PersonId = id, AmountMultiplier = Multipliers.GetRandomItem()};
    }

    private void PerformMealLevelMutation(GroupMeal meal, RecipeGroupSplit recipe, GroupDietIndividual individual)
    {
      const int maxRecipes = 4;
      const int minRecipes = 1;

      var includedRecipes = individual.GroupDiet.Meals.SelectMany(m => m.Recipes).Select(r => r.Recipe).ToList();

      switch (RandomMutationType())
      {
        case MutationType.Remove:
          meal.Recipes.Remove(recipe);
          if (meal.Recipes.Count < minRecipes)
            meal.Recipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
          break;
        case MutationType.Add:
          meal.Recipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
          if (meal.Recipes.Count > maxRecipes)
            meal.Recipes.Remove(recipe);
          break;
        case MutationType.Replace:
          meal.Recipes.Remove(recipe);
          meal.Recipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));
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

    private RecipeGroupSplit GetRanodmNotInvlolvedRecipe(List<Recipe> includedRecipes)
    {
      var recipeGroupSplit = new RecipeGroupSplit(_groupSize);

      do
      {
        recipeGroupSplit.Recipe = _recipes.GetRandomItem();
      } while (includedRecipes.Contains(recipeGroupSplit.Recipe));

      return recipeGroupSplit;
    }
  }

  internal enum MutationType
  {
    Remove,
    Add,
    Replace
  }
}
