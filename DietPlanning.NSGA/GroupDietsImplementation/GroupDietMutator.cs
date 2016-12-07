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
    private readonly GroupDietCorrector _corrector;


    public GroupDietMutator(Random random, List<Recipe> recipes, int groupSize, GroupDietCorrector corrector)
    {
      _random = random;
      _recipes = recipes;
      _groupSize = groupSize;
      _corrector = corrector;
    }

    public void Mutate(Individual individual, double mutationProbability)
    {
      var dietIndividual = individual as GroupDietIndividual;
      var meals = dietIndividual.GroupDiet.Meals.Select(m => m);

      //foreach (var meal in meals)
      //{
      //  for (var i = meal.Recipes.Count - 1; i >= 0; i--)
      //  {
      //    if (_random.NextDouble() < mutationProbability)
      //    {
      //      PerformMealLevelMutation(meal, meal.Recipes[i], (GroupDietIndividual)individual);
      //    }
      //    else
      //    {
      //      PerformRecipesLevelMutation(meal, mutationProbability);
      //    }
      //  }
      //}
      foreach (var meal in meals)
      {
        for (var i = meal.Recipes.Count - 1; i >= 0; i--)
        {
          PerformRecipesLevelMutation(meal, mutationProbability);
        }
      }

     // _corrector.ApplyCorrection(dietIndividual.GroupDiet);
    }

    private void PerformRecipesLevelMutation(GroupMeal meal, double mutationProbability)
    {
      if (_random.NextDouble() < mutationProbability)
      {
        //add new split
        var split = new RecipeGroupSplit {Recipe = _recipes.GetRandomItem()};
        var adjustment = new RecipeAdjustment
        {
          AmountMultiplier = RecipeGroupSplit.Multipliers.GetRandomItem(),
          PersonId = _random.Next(_groupSize)
        };

        var duplicate = meal.Recipes.FirstOrDefault(r => r.Recipe == split.Recipe);
        if (duplicate == null)
        {
          meal.Recipes.Add(split);
        }
        else if (duplicate.Adjustments.All(a => a.PersonId != adjustment.PersonId))
        {
          duplicate.Adjustments.Add(adjustment);
        }
      }
      
      foreach (var recipeGroupSplit in meal.Recipes)
      {
        if (_random.NextDouble() < mutationProbability)
        {
          //add adjustment to split
          if (recipeGroupSplit.Adjustments.Count < _groupSize)
            recipeGroupSplit.Adjustments.Add(GetRandomMissingAdjustment(recipeGroupSplit.Adjustments));
        }

        for (var adjIndex = recipeGroupSplit.Adjustments.Count - 1; adjIndex >= 0; adjIndex--)
        {
          if (!(_random.NextDouble() < mutationProbability)) continue;
          if (_random.NextDouble() < 0.5)
          {
            //remove adjustment from split
            recipeGroupSplit.Adjustments.RemoveAt(adjIndex);
          }
          else
          {
            //change multiplier
            recipeGroupSplit.Adjustments[adjIndex].AmountMultiplier = RecipeGroupSplit.Multipliers.GetRandomItem();
          }
        }
      }
      meal.Recipes.RemoveAll(r => !r.Adjustments.Any());
    }

    private RecipeAdjustment GetRandomMissingAdjustment(List<RecipeAdjustment> adjustments)
    {
      int id;

      do
      {
        id = _random.Next(_groupSize);
      } while (adjustments.Any(ad => ad.PersonId == id));

      return new RecipeAdjustment { PersonId = id, AmountMultiplier = RecipeGroupSplit.Multipliers.GetRandomItem() };
    }

    private void PerformMealLevelMutation(GroupMeal meal, RecipeGroupSplit recipe, GroupDietIndividual individual)
    {
      const int maxRecipes = 5;
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
