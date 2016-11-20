using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.NSGA.DietImplementation;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class DietController : Controller
  {
    public ActionResult ShowDiets()
    {
      var configProvider = new ConfigurationProvider();
      var nsgaSolverFactory = new NsgaSolverFactory(configProvider, new Random());
      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var requirementsProvider = new RequirementsProvider();

      var dietRequirements = requirementsProvider.GetRequirements(GetPersonalData(), 5);
      var nsgaSolver = nsgaSolverFactory.GetDietSolver(recipeGenerator.GetRecipes(), dietRequirements);

      var nsgaResult = nsgaSolver.Solve();

      var dietsViewModel = CreateDietsViewModel(nsgaResult, dietRequirements);

      return View(dietsViewModel);
    }

    public ActionResult ShowDays()
    {
      var configProvider = new ConfigurationProvider();
      var nsgaSolverFactory = new NsgaSolverFactory(configProvider, new Random());
      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var requirementsProvider = new RequirementsProvider();

      var dietRequirements = requirementsProvider.GetRequirements(GetPersonalData(), 5);
      var nsgaSolver = nsgaSolverFactory.GetDailyDietsSolver(recipeGenerator.GetRecipes(), dietRequirements);

      var nsgaResult = nsgaSolver.Solve();

      var dietsViewModel = CreateDailyDietsResultViewModel(nsgaResult, dietRequirements);

      return View(dietsViewModel);
    }

    private DailyDietsResultViewModel CreateDailyDietsResultViewModel(List<List<Individual>> nsgaResult, DietRequirements dietRequirements)
    {
      var dietAnalyzer = new DietAnalyzer();
      var viewModel = new DailyDietsResultViewModel {DietRequirements = dietRequirements };

      foreach (var individual in nsgaResult.SelectMany(r => r))
      {
        var dayIndividual = (DayIndividual) individual;

        var dailyDietViewModel = new DailyDietViewModel();

        dailyDietViewModel.Evaluations.AddRange(dayIndividual.Evaluations);
        dailyDietViewModel.DietSummary = dietAnalyzer.SummarizeDaily(dayIndividual.DailyDiet);
        dailyDietViewModel.DailyDiet = dayIndividual.DailyDiet;

        viewModel.DailyDietViewModels.Add(dailyDietViewModel);
      }

      return viewModel;
    }

    private DietsViewModel CreateDietsViewModel(List<List<Individual>> nsgaResult, DietRequirements dietRequirements)
    {
      var dietsViewModel = new DietsViewModel {DietRequirements = dietRequirements };
      var dietAnalyzer = new DietAnalyzer();

      foreach (var individual in nsgaResult.First())
      {
        var dietViewModel = new DietViewModel();

        dietViewModel.Evaluations.AddRange(individual.Evaluations);
        dietViewModel.DailyDiets = GetDailyDietsViewModels(((DietIndividual)individual).Diet, dietAnalyzer);
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
   
    private PersonalData GetPersonalData()
    {
      return new PersonalData
      {
        Age = 25,
        Gender = Gender.Male,
        Height = 185,
        Weight = 85,
        Pal = 1.5
      };
    }
  }
}
