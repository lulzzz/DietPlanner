using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class DietEvaluator : IEvaluator<Diet>
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietSummary _targetDailyDiet;

    public DietEvaluator(DietAnalyzer dietAnalyzer, DietSummary targetDailyDiet)
    {
      _dietAnalyzer = dietAnalyzer;
      _targetDailyDiet = targetDailyDiet;
    }

    public void Evaluate(List<Individual<Diet>> individuals)
    {
      individuals.ForEach(individual => Evaluate(individual));
    }

    public void Evaluate(Individual<Diet> individual)
    {
      var recipes = individual.Solution.GetRecipes();

      individual.Evaluations.Clear();

      individual.Evaluations.Add(EvaluateMacro(individual.Solution, _targetDailyDiet));
      individual.Evaluations.Add(EvaluateCost(recipes));
     // individual.Evaluations.Add(EvaluatePreparationTime(recipes));
     // individual.Evaluations.Add(EvaluateVariety(recipes));
      //todo preferences
    }

    private Evaluation EvaluateVariety(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Variety,
        Score = recipes.GroupBy(recipe => recipe.Group).Count() +
                recipes.SelectMany(recipe => recipe.Ingredients).GroupBy(ingredient => ingredient.Food.Group).Count()
      };
    }

    private Evaluation EvaluatePreparationTime(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.PreparationTime,
        Score = -1.0 * recipes.Select(recipe => recipe.PreparationTimeInMinutes).Sum()
      };
    }

    private Evaluation EvaluateCost(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Cost,
        Score = -1.0 * recipes.Select(recipe => recipe.Cost).Sum()
      };
    }

    private Evaluation EvaluateMacro(Diet diet, DietSummary targetDailyDiet)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Score = diet.DailyDiets.Select(dailyDiet => EvaluateDailyMacro(dailyDiet, targetDailyDiet)).Sum()
      }; 
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
