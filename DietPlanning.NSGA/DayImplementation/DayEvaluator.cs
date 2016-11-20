using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayEvaluator : IEvaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietRequirements _dietRequirements;

    public DayEvaluator(DietAnalyzer dietAnalyzer, DietRequirements dietRequirements)
    {
      _dietAnalyzer = dietAnalyzer;
      _dietRequirements = dietRequirements;
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

    private double EvaluateDailyMacro(DailyDiet dailyDiet, out bool feasible)
    {
      feasible = true;
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance = 0.0 +
                     Math.Abs(_dietRequirements.ProteinRange.GetDistanceToRange(dailySummary.Proteins)) +
                     Math.Abs(_dietRequirements.FatRange.GetDistanceToRange(dailySummary.Fat)) +
                     Math.Abs(_dietRequirements.CarbohydratesRange.GetDistanceToRange(dailySummary.Carbohydrates));
                     Math.Abs(_dietRequirements.Calories - dailySummary.Calories);

      if (_dietRequirements.ProteinRange.IsInRange(dailySummary.Proteins) ||
          _dietRequirements.FatRange.IsInRange(dailySummary.Fat) ||
          _dietRequirements.CarbohydratesRange.IsInRange(dailySummary.Carbohydrates))
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
