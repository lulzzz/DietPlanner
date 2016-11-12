using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DietImplementation
{
  public class DietEvaluator : IEvaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietSummary _targetDailyDiet;

    public DietEvaluator(DietAnalyzer dietAnalyzer, DietSummary targetDailyDiet)
    {
      _dietAnalyzer = dietAnalyzer;
      _targetDailyDiet = targetDailyDiet;
    }

    public void Evaluate(List<Individual> individuals)
    {
      individuals.ForEach(individual => Evaluate((DietIndividual)individual));
    }

    public void Evaluate(Individual individual)
    {
      var recipes = DietHelper.GetRecipes(((DietIndividual) individual).Diet);

      individual.Evaluations.Clear();

      individual.Evaluations.Add(EvaluateMacro(((DietIndividual)individual).Diet));
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

    private Evaluation EvaluateMacro(Core.DomainObjects.Diet diet)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Score = diet.DailyDiets.Select(dailyDiet => EvaluateDailyMacro(dailyDiet)).Sum()
      }; 
    }

    private double EvaluateDailyMacro(DailyDiet dailyDiet)
    {
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance =
        Math.Abs(_targetDailyDiet.Proteins - dailySummary.Proteins) +
        Math.Abs(_targetDailyDiet.Fat - dailySummary.Fat) +
        Math.Abs(_targetDailyDiet.Calories - dailySummary.Calories) +
        Math.Abs(_targetDailyDiet.Carbohydrates - dailySummary.Carbohydrates);

      return distance * -1;
      //distance > 0 
      //? 1 / (distance) 
      //: 1;
    }
  }
}
