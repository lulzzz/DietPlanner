using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayEvaluator : IEvaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietSummary _targetDailyDiet;

    public DayEvaluator(DietAnalyzer dietAnalyzer, DietSummary targetDailyDiet)
    {
      _dietAnalyzer = dietAnalyzer;
      _targetDailyDiet = targetDailyDiet;
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

      var macroEv = EvaluateMacro(dailyDiet);
      individual.Evaluations.Add(macroEv);
      individual.Evaluations.Add(EvaluateCost(recipes));
      individual.Evaluations.Add(EvaluatePreparationTime(recipes));

      if (macroEv.Score > 150)
      {
        individual.IsFeasible = false;
      }
      
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

    private Evaluation EvaluateMacro(DailyDiet dailyDiet)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Direction = Direction.Minimize,
        Score = EvaluateDailyMacro(dailyDiet)
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

    private double EvaluateDailyMacro(DailyDiet dailyDiet)
    {
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance =
        Math.Abs(_targetDailyDiet.Proteins - dailySummary.Proteins) +
        Math.Abs(_targetDailyDiet.Fat - dailySummary.Fat) +
        Math.Abs(_targetDailyDiet.Calories - dailySummary.Calories) +
        Math.Abs(_targetDailyDiet.Carbohydrates - dailySummary.Carbohydrates);

      return distance;
    }
  }
}
