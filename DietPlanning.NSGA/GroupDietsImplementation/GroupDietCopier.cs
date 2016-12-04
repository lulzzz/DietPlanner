using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DietPlanning.Core.GroupDiets;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public static class GroupDietCopier
  {
    public static GroupDiet Copy(GroupDiet source)
    {
      var copy = new GroupDiet();

      foreach (var sourceMeal in source.Meals)
      {
        copy.Meals.Add(CopyMeal(sourceMeal));
      }

      return copy;
    }

    private static GroupMeal CopyMeal(GroupMeal sourceMeal)
    {
      return new GroupMeal
      {
        MealType = sourceMeal.MealType,
        Recipes = CopyRecipes(sourceMeal.Recipes)
      };
    }

    private static List<RecipeGroupSplit> CopyRecipes(List<RecipeGroupSplit> sourceMealRecipes)
    {
      var recipes = new List<RecipeGroupSplit>();

      foreach (var sourceMealRecipe in sourceMealRecipes)
      {
        recipes.Add(new RecipeGroupSplit
        {
          Recipe = sourceMealRecipe.Recipe,
          Adjustments = CopyAdjustments(sourceMealRecipe.Adjustments)
        });
      }

      return recipes;
    }

    private static List<RecipeAdjustment> CopyAdjustments(List<RecipeAdjustment> adjustments)
    {
      var copiedAdjustments = new List<RecipeAdjustment>();

      foreach (var recipeAdjustment in adjustments)
      {
        copiedAdjustments.Add(new RecipeAdjustment
        {
          AmountMultiplier = recipeAdjustment.AmountMultiplier,
          PersonId = recipeAdjustment.PersonId
        });
      }

      return copiedAdjustments;
    }
  }
}
