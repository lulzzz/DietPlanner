using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.NSGA.DietImplementation
{
  public class DietEvaluator : IEvaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;
    private readonly DietRequirements _dietRequirements;

    public DietEvaluator(DietAnalyzer dietAnalyzer, DietRequirements dietRequirements)
    {
      _dietAnalyzer = dietAnalyzer;
      _dietRequirements = dietRequirements;
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
        Direction = Direction.Maximize,
        Score = recipes.GroupBy(recipe => recipe.Group).Count() +
                recipes.SelectMany(recipe => recipe.Ingredients).GroupBy(ingredient => ingredient.Food.Group).Count()
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

    private Evaluation EvaluateCost(List<Recipe> recipes)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Cost,
        Direction = Direction.Minimize,
        Score = recipes.Select(recipe => recipe.Cost).Sum()
      };
    }

    private Evaluation EvaluateMacro(Core.DomainObjects.Diet diet)
    {
      return new Evaluation
      {
        Type = ObjectiveType.Macro,
        Direction = Direction.Minimize,
        Score = diet.DailyDiets.Select(dailyDiet => EvaluateDailyMacro(dailyDiet)).Sum()
      }; 
    }

    private double EvaluateDailyMacro(DailyDiet dailyDiet)
    {
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);
      //todo fpr grpup of ppl
      var distance = 0.0 +
                     Math.Abs(_dietRequirements.ProteinRange.GetDistanceToRange(dailySummary.Proteins)) +
                     Math.Abs(_dietRequirements.FatRange.GetDistanceToRange(dailySummary.Fat)) +
                     Math.Abs(_dietRequirements.CarbohydratesRange.GetDistanceToRange(dailySummary.Carbohydrates));

      return distance;
    }
  }
}
