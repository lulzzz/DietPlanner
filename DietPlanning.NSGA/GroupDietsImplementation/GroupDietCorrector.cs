using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;
using Tools;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public class GroupDietCorrector
  {
    private readonly GroupDietAnalyzer _dietAnalyzer;
    private readonly List<PersonalData> _personalData;
    private readonly List<Recipe> _recipes;

    public GroupDietCorrector(GroupDietAnalyzer dietAnalyzer, List<PersonalData> personalData, List<Recipe> recipes)
    {
      _dietAnalyzer = dietAnalyzer;
      _personalData = personalData;
      _recipes = recipes;
    }

    public void ApplyCorrection(GroupDiet diet)
    {
      var includedRecipes = diet.Meals.SelectMany(m => m.Recipes).Select(r => r.Recipe).ToList();
      var differences = GetDifferences(diet);
      var mealDifferences = GetMealDifferences(diet);
      while (differences.All(d => d.Calories > 200))
      {
        var totalMealDifference = new List<double>();

        for (int i = 0; i < mealDifferences.First().Count; i++)
        {
          totalMealDifference.Add(mealDifferences.Select(md => md[i]).Sum());
        }

        var mealWithBiggestDifference = totalMealDifference.IndexOf(totalMealDifference.Min());
        var recipe = GetRanodmNotInvlolvedRecipe(includedRecipes);
        
        diet.Meals[mealWithBiggestDifference].Recipes.Add(recipe);
        includedRecipes.Add(recipe.Recipe);
        differences = GetDifferences(diet);
        mealDifferences = GetMealDifferences(diet);
      }

      foreach (var id in _personalData.Select(pd => pd.Id))
      {
        for (var i = 0; i < mealDifferences[id].Count; i++)
        {
          if (mealDifferences[id][i] < 100)
          {
            var adjustments = diet.Meals[i].Recipes.SelectMany(r => r.Adjustments).Where(a => a.PersonId == id)
              .Where(a => RecipeGroupSplit.Multipliers.Last() > a.AmountMultiplier).ToList();

            if (adjustments.Any())
            {
              IncreaseAjustment(adjustments.GetRandomItem());
            }

          }
          else if (mealDifferences[id][i] > -100)
          {
            var adjustments = diet.Meals[i].Recipes.SelectMany(r => r.Adjustments).Where(a => a.PersonId == id)
             .Where(a => RecipeGroupSplit.Multipliers.First() < a.AmountMultiplier).ToList();

            if (adjustments.Any())
            {
              DecreaseAjustment(adjustments.GetRandomItem());
            }
          }
        }
      }

    //  var idsWithNotEnoughCalories = differences.Where(d => d.Calories < -100).Select(d => differences.IndexOf(d)).ToList();
     // var idsWithToMuchCalories = differences.Where(d => d.Calories > 0).Select(d => differences.IndexOf(d)).ToList();
    }

    private void IncreaseAjustment(RecipeAdjustment recipeAdjustment)
    {
      recipeAdjustment.AmountMultiplier = RecipeGroupSplit.Multipliers.Last();
    }

    private void DecreaseAjustment(RecipeAdjustment recipeAdjustment)
    {
      recipeAdjustment.AmountMultiplier = RecipeGroupSplit.Multipliers.First();
    }

    private List<List<double>> GetMealDifferences(GroupDiet diet)
    {
      var mealDifferences = new List<List<double>>();

      foreach (var personalData in _personalData)
      {
        var personMealDifferences = new List<double>();
        var dietSummary = _dietAnalyzer.SummarizeForPerson(diet, personalData.Id);

        for (int i = 0; i < dietSummary.CaloriesPerMeal.Count; i++)
        {
          personMealDifferences.Add(personalData.Requirements.MealCaloriesSplit[i].GetDistanceToRange(dietSummary.CaloriesPerMeal[i]));
        }
        mealDifferences.Add(personMealDifferences);
      }
      return mealDifferences;
    }

    private List<NutritionValues> GetDifferences(GroupDiet diet)
    {
      var allDifferences = new List<NutritionValues>();

      foreach (var personalData in _personalData)
      {
        var dietSummary = _dietAnalyzer.SummarizeForPerson(diet, personalData.Id);

        allDifferences.Add(new NutritionValues
        {
          Calories = personalData.Requirements.CaloriesAllowedRange.GetDistanceToRange(dietSummary.NutritionValues.Calories),
          Fat = personalData.Requirements.FatRange.GetDistanceToRange(dietSummary.NutritionValues.Fat),
          Proteins = personalData.Requirements.FatRange.GetDistanceToRange(dietSummary.NutritionValues.Proteins),
          Carbohydrates = personalData.Requirements.FatRange.GetDistanceToRange(dietSummary.NutritionValues.Carbohydrates)
        });
      }

      return allDifferences;
    }

    private RecipeGroupSplit GetRanodmNotInvlolvedRecipe(List<Recipe> includedRecipes)
    {
      var recipeGroupSplit = new RecipeGroupSplit(_personalData.Count);

      do
      {
        recipeGroupSplit.Recipe = _recipes.GetRandomItem();
      } while (includedRecipes.Contains(recipeGroupSplit.Recipe));

      return recipeGroupSplit;
    }
  }
}
