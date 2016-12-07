using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public class GroupDietEvaluator : IEvaluator
  {
    private readonly GroupDietAnalyzer _dietAnalyzer;
    private readonly List<PersonalData> _personalData;

    public GroupDietEvaluator(List<PersonalData> personalData, GroupDietAnalyzer dietAnalyzer)
    {
      _dietAnalyzer = dietAnalyzer;
      _personalData = personalData;
    }

    public void Evaluate(List<Individual> individuals)
    {
      individuals.ForEach(individual => Evaluate((GroupDietIndividual)individual));
    }

    public void Evaluate(Individual individual)
    {
      var groupDiet = ((GroupDietIndividual) individual).GroupDiet;
      var recipes = groupDiet.Meals.SelectMany(meal => meal.Recipes).ToList();

      individual.Evaluations.Clear();
      individual.IsFeasible = true;

      var macroEv = EvaluateMacro(groupDiet, out individual.IsFeasible);
      individual.Evaluations.Add(macroEv);
      individual.Evaluations.Add(EvaluateCost(recipes));
      //  individual.Evaluations.Add(EvaluatePreparationTime(recipes));
      //individual.Evaluations.Add(EvaluatePreferences(recipes));

      //todo preferences
    }
    
    private Evaluation EvaluateCost(List<RecipeGroupSplit> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Cost,
        Direction = Direction.Minimize,
        Score = (double)recipes.Select(r => r.Recipe.Cost * r.Adjustments.Count).Sum() / recipes.SelectMany(r => r.Adjustments).Count()
      };
    }

    private Evaluation EvaluateMacro(GroupDiet groupDiet, out bool feasible)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Direction = Direction.Minimize,
        Score = EvaluateDailyMacro(groupDiet, out feasible)
      };
    }

    private Evaluation EvaluatePreparationTime(List<RecipeGroupSplit> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.PreparationTime,
        Direction = Direction.Minimize,
        Score = recipes.Select(recipe => recipe.Recipe.PreparationTimeInMinutes).Sum()
      };
    }

    private Evaluation EvaluatePreferences(List<RecipeGroupSplit> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Preferences,
        Direction = Direction.Maximize,
        Score = GetPreferencesScore(recipes)
      };
    }

    private double GetPreferencesScore(List<RecipeGroupSplit> recipes)
    {
      var score = 0.0;
      foreach (var personalData in _personalData)
      {
        var recipesForPerson = recipes.RecipesForPerson(personalData.Id);
        var mainCategories = recipesForPerson.Select(r => r.MainCategory).ToList();
        var subCategories = recipesForPerson.Select(r => r.SubCategory).ToList();

        foreach (var categoryPreference in personalData.Preferences.CategoryPreferences)
        {
          if (Math.Abs(categoryPreference.Preference) < 0.1)
            continue;

          var categorySet = categoryPreference.CategoryLevel == CategoryLevel.MainCategory
            ? mainCategories
            : subCategories;

          score += categorySet.Count(mainCat => mainCat == categoryPreference.Name)*categoryPreference.Preference;
        }
      }

      return score;
    }

    private double EvaluateDailyMacro(GroupDiet diet, out bool feasible)
    {
      var distance = 0.0;

      feasible = true;
      foreach (var personalData in _personalData)
      {
        var dailySummary = _dietAnalyzer.SummarizeForPerson(diet, personalData.Id);

        var distanceForPerson =
          Math.Abs(personalData.Requirements.ProteinRange.GetDistanceToRange(dailySummary.NutritionValues.Proteins)) +
          Math.Abs(personalData.Requirements.FatRange.GetDistanceToRange(dailySummary.NutritionValues.Fat)) +
          Math.Abs(personalData.Requirements.CarbohydratesRange.GetDistanceToRange(dailySummary.NutritionValues.Carbohydrates));
        
        ////todo make sure about last part
        //if (!personalData.Requirements.CaloriesAllowedRange.IsInRange(dailySummary.NutritionValues.Calories) || distanceForPerson > 0)
        //{
        //  feasible = false;
        //}

        if (!personalData.Requirements.CaloriesAllowedRange.IsInRange(dailySummary.NutritionValues.Calories))
        {
          feasible = false;
        }


        for (var mealIndex = 0; mealIndex < dailySummary.CaloriesPerMeal.Count; mealIndex++)
        {
          distanceForPerson +=
            Math.Abs(
              personalData.Requirements
                .MealCaloriesSplit[mealIndex]
                .GetDistanceToRange(dailySummary.CaloriesPerMeal[mealIndex]));
        }

        distance += distanceForPerson;
      }

      return distance;
    }
  }
}
