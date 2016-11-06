using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class Evaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;

    public Evaluator(DietAnalyzer dietAnalyzer)
    {
      _dietAnalyzer = dietAnalyzer;
    }

    public void Evaluate(Individual individual, DietSummary targetDailyDiet)
    {
      var recipes = individual.Diet.GetRecipes();

      individual.Evaluation.Macro = EvaluateMacro(individual.Diet, targetDailyDiet);
      individual.Evaluation.Cost = EvaluateCost(recipes);
      individual.Evaluation.PreparationTime = EvaluatePreparationTime(recipes);
      individual.Evaluation.Variety = EvaluateVariety(recipes);
      //todo preferences
    }

    private double EvaluateVariety(List<Recipe> recipes)
    {
      return recipes.GroupBy(recipe => recipe.Group).Count() + 
        recipes.SelectMany(recipe => recipe.Ingredients).GroupBy(ingredient => ingredient.Food.Group).Count();
    }

    private double EvaluatePreparationTime(List<Recipe> recipes)
    {
      return -1.0 * recipes.Select(recipe => recipe.PreparationTimeInMinutes).Sum();
    }

    private double EvaluateCost(List<Recipe> recipes)
    {
      return -1.0 * recipes.Select(recipe => recipe.Cost).Sum();
    }

    private double EvaluateMacro(Diet diet, DietSummary targetDailyDiet)
    {
      return diet.DailyDiets.Select(dailyDiet => EvaluateDailyMacro(dailyDiet, targetDailyDiet)).Sum() / diet.DailyDiets.Count;
    }

    private double EvaluateDailyMacro(DailyDiet dailyDiet, DietSummary targetDailyDiet)
    {
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance =
        Math.Abs(targetDailyDiet.Proteins - dailySummary.Proteins) +
        Math.Abs(targetDailyDiet.Fat - dailySummary.Fat) +
        Math.Abs(targetDailyDiet.Calories - dailySummary.Calories) +
        Math.Abs(targetDailyDiet.Carbohydrates - dailySummary.Carbohydrates);

      return distance * -1;
      //distance > 0 
      //? 1 / (distance) 
      //: 1;
    }
  }
}
