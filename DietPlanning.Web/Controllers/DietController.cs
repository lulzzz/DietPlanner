using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DataProviders.RandomData;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class DietController : Controller
  {
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
