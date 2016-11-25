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
    const string SettingsKey = "Settings";
    const string PersonalDataKey = "PersonalData";

    public ActionResult ShowDays()
    {
      if (!TempData.ContainsKey(SettingsKey))
        return RedirectToAction("Edit", "Settings");

      var config = ((SettingsViewModel) TempData.Peek(SettingsKey)).NsgaConfiguration;

      var nsgaSolverFactory = new NsgaSolverFactory(config, new Random());
      var recipeGenerator = new RandomRecipeProvider(new Random(), 500, new FoodDatabaseProvider().GetFoods());
      var requirementsProvider = new RequirementsProvider();

      var dietRequirements = requirementsProvider.GetRequirements(GetPersonalData(), 5);
      var nsgaSolver = nsgaSolverFactory.GetDailyDietsSolver(recipeGenerator.GetRecipes(), dietRequirements);

      var nsgaResult = nsgaSolver.Solve();

      var dietsViewModel = CreateDailyDietsResultViewModel(nsgaResult, dietRequirements);

      dietsViewModel.DailyDietViewModels = dietsViewModel.DailyDietViewModels.OrderBy(d => d.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).ToList();

      return View(dietsViewModel);
    }

    [HttpGet]
    public ActionResult PersonalData()
    {
      var personalData = GetPersonalData();

      return View(personalData);
    }

    [HttpPost]
    public ActionResult PersonalData(PersonalData personalData)
    {
      TempData[PersonalDataKey] = personalData;

      return View(personalData);
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
      if (!TempData.ContainsKey(PersonalDataKey))
      {
        TempData[PersonalDataKey] = new PersonalData
        {
          Age = 25,
          Gender = Gender.Male,
          Height = 185,
          Weight = 85,
          Pal = 1.5
        };
      }

      return TempData.Peek(PersonalDataKey) as PersonalData;
    }
  }
}
