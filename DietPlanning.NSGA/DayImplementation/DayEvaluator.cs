using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayEvaluator : IEvaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietRequirements _dietRequirements;
    private readonly DietPreferences _dietPreferences;

    public DayEvaluator(DietAnalyzer dietAnalyzer, DietRequirements dietRequirements, DietPreferences dietPreferences)
    {
      _dietAnalyzer = dietAnalyzer;
      _dietRequirements = dietRequirements;
      _dietPreferences = dietPreferences;
    }

    public void Evaluate(List<Individual> individuals)
    {
      individuals.ForEach(individual => Evaluate((DayIndividual)individual));
    }

    public void Evaluate(Individual individual)
    {
      var dailyDiet = ((DayIndividual) individual).DailyDiet;
      var recipes = dailyDiet.Meals.SelectMany(meal => meal.Receipes).ToList();

      individual.Evaluations.Clear();
      individual.IsFeasible = true;

      var macroEv = EvaluateMacro(dailyDiet, out individual.IsFeasible);
      individual.Evaluations.Add(macroEv);
      individual.Evaluations.Add(EvaluateCost(recipes));
      individual.Evaluations.Add(EvaluatePreparationTime(recipes));
      individual.Evaluations.Add(EvaluatePreferences(recipes));
      
      //todo preferences
    }
    
    private Evaluation EvaluateCost(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Cost,
        Direction = Direction.Minimize,
        Score = recipes.Select(recipe => recipe.Cost).Sum()
      };
    }

    private Evaluation EvaluateMacro(DailyDiet dailyDiet, out bool feasible)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Direction = Direction.Minimize,
        Score = EvaluateDailyMacro(dailyDiet, out feasible)
      };
    }

    private Evaluation EvaluatePreparationTime(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.PreparationTime,
        Direction = Direction.Minimize,
        Score = recipes.Select(recipe => recipe.PreparationTimeInMinutes).Sum()
      };
    }

    private Evaluation EvaluatePreferences(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Preferences,
        Direction = Direction.Maximize,
        Score = GetPreferencesScore(recipes)
      };
    }

    private double GetPreferencesScore(List<Recipe> recipes)
    {
      var mainCategories = recipes.Select(r => r.MainCategory).ToList();
      var subCategories = recipes.Select(r => r.SubCategory).ToList();

      var score = 0.0;

      //foreach (var categoryPreference in _dietPreferences.CategoryPreferences)
      //{
      //  if(Math.Abs(categoryPreference.Preference) < 0.1)
      //    continue;

      //  var categorySet = categoryPreference.CategoryLevel == CategoryLevel.MainCategory
      //    ? mainCategories
      //    : subCategories;

      //  score += categorySet.Count(mainCat => mainCat == categoryPreference.Name) * categoryPreference.Preference;
      //}

      return score;
    }

    private double EvaluateDailyMacro(DailyDiet dailyDiet, out bool feasible)
    {
      feasible = true;
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance = Math.Abs(_dietRequirements.ProteinRange.GetDistanceToRange(dailySummary.NutritionValues.Proteins)) +
                     Math.Abs(_dietRequirements.FatRange.GetDistanceToRange(dailySummary.NutritionValues.Fat)) +
                     Math.Abs(_dietRequirements.CarbohydratesRange.GetDistanceToRange(dailySummary.NutritionValues.Carbohydrates));

      if (! _dietRequirements.CaloriesAllowedRange.IsInRange(dailySummary.NutritionValues.Calories) || distance > 0)
      {
        feasible = false;
      }

      for (var mealIndex = 0; mealIndex < dailySummary.CaloriesPerMeal.Count; mealIndex++)
      {
        distance +=
          Math.Abs(
            _dietRequirements
            .MealCaloriesSplit[mealIndex]
            .GetDistanceToRange(dailySummary.CaloriesPerMeal[mealIndex]));
      }

      return distance;
    }
  }
}
