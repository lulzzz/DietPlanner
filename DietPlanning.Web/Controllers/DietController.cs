using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.Core.DomainObjects;
using DietPlanning.NSGA;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class DietController : Controller
  {
    public ActionResult ShowDiets()
    {
      const int tournamentSize = 2;
      
      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var nsgaSolver = CreateNsgaSolver(recipeGenerator.GetRecipes(), tournamentSize);

      var targetDiet = GetTargetDiet();
      var nsgaResult = nsgaSolver.Solve(targetDiet);

      var dietsViewModel = CreateDietsViewModel(nsgaResult, targetDiet);

      return View(dietsViewModel);
    }

    private DietsViewModel CreateDietsViewModel(List<List<Individual>> nsgaResult, DietSummary targetDiet)
    {
      var dietsViewModel = new DietsViewModel {TargetDiet = targetDiet};
      var dietAnalyzer = new DietAnalyzer();

      foreach (var individual in nsgaResult.First())
      {
        var dietViewModel = new DietViewModel();

        dietViewModel.Evaluations.AddRange(individual.Evaluations);
        dietViewModel.DailyDiets = GetDailyDietsViewModels(individual.Diet, dietAnalyzer);
        dietViewModel.AverageDietSummary = GetAverageDietSummary(dietViewModel.DailyDiets, dietAnalyzer);

        dietsViewModel.Diets.Add(dietViewModel);
      }

      return dietsViewModel;
    }

    private DietSummary GetAverageDietSummary(List<DailyDietViewModel> dailyDiets, DietAnalyzer dietAnalyzer)
    {
      return new DietSummary
      {
        Calories = dailyDiets.Select(diet => diet.DietSummary.Calories).Sum() / dailyDiets.Count,
        Carbohydrates = dailyDiets.Select(diet => diet.DietSummary.Carbohydrates).Sum() / dailyDiets.Count,
        Fat = dailyDiets.Select(diet => diet.DietSummary.Fat).Sum() / dailyDiets.Count,
        Proteins = dailyDiets.Select(diet => diet.DietSummary.Proteins).Sum() / dailyDiets.Count
      };
    }

    private List<DailyDietViewModel> GetDailyDietsViewModels(Diet diet, DietAnalyzer dietAnalyzer)
    {
      var dailyDietsViewModels = new List<DailyDietViewModel>();

      foreach (var dailyDiet in diet.DailyDiets)
      {
        dailyDietsViewModels.Add(new DailyDietViewModel
          {
            DietSummary = dietAnalyzer.SummarizeDaily(dailyDiet),
            DailyDiet = dailyDiet
          }
        );
      }

      return dailyDietsViewModels;
    }
    
    private NsgaSolver CreateNsgaSolver(List<Recipe> recipes, int tournamentSize)
    {
      return new NsgaSolver(
        new Sorter(),
        new PopulationInitializer(new Random(), recipes),
        new Evaluator(new DietAnalyzer()),
        new TournamentSelector(new CrowdedDistanceComparer(), tournamentSize, new Random()),
        new DayCrossOver(new Random()), new Mutator(new Random(), recipes));
    }

    private DietSummary GetTargetDiet()
    {
      return new DietSummary
      {
        Calories = 3002,
        Fat = 101,
        Carbohydrates = 338,
        Proteins = 188
      };
    }
  }
}
