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

      var idsWithNotEnoughCalories = differences.Where(d => d.Calories < 0).Select(d => differences.IndexOf(d)).ToList();
      var idsWithToMuchCalories = differences.Where(d => d.Calories > 0).Select(d => differences.IndexOf(d)).ToList();

//      diet.Meals.GetRandomItem().Recipes.Add(GetRanodmNotInvlolvedRecipe(includedRecipes));

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
