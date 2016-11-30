using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;

namespace DietPlanning.Web.Controllers
{
  public class DietController : Controller
  {
    private readonly RequirementsProvider _requirementsProvider;
    private readonly NsgaSolverFactory _nsgaSolverFactory;
    private readonly IRecipeProvider _recipeProvider;

    public DietController(RequirementsProvider requirementsProvider, NsgaSolverFactory nsgaSolverFactory, IRecipeProvider recipeProvider)
    {
      _requirementsProvider = requirementsProvider;
      _nsgaSolverFactory = nsgaSolverFactory;
      _recipeProvider = recipeProvider;
    }

    public ActionResult ShowDays()
    {
      var dietsViewModel = TempData.GetDailyDietsResultViewModel();

      if (dietsViewModel == null)
      {
        return RedirectToAction("GenerateDiets");
      }

      return View(dietsViewModel);
    }

    public ActionResult GenerateDiets()
    {
      var dietRequirements = _requirementsProvider.GetRequirements(TempData.GetPersonalData(), 5);
      var nsgaSolver = _nsgaSolverFactory.GetDailyDietsSolver(TempData.GetSettings().NsgaConfiguration, _recipeProvider.GetRecipes(), dietRequirements);

      var nsgaResult = nsgaSolver.Solve();
      TempData.SaveLog(nsgaResult.Log);

      var dietsViewModel = CreateDailyDietsResultViewModel(nsgaResult.Fronts, dietRequirements);
      TempData.SaveDailyDietsResultViewModel(dietsViewModel);

      return RedirectToAction("ShowDays");
    }

    [HttpGet]
    public ActionResult PersonalData()
    {
      return View(TempData.GetPersonalData());
    }

    [HttpPost]
    public ActionResult PersonalData(PersonalData personalData)
    {
      TempData.SavePersonalData(personalData);

      return View(personalData);
    }

    [HttpGet]
    public ActionResult Preferences()
    {
      return View(TempData.GetPreferences());
    }

    [HttpPost]
    public ActionResult Preferences(PreferencesViewModel preferences)
    {
      return View(TempData.SavePreferences(preferences));
    }

    [HttpGet]
    public JsonResult GetFoods(string term)
    {
      var foods = new FoodDatabaseProvider().GetFoods().ToList();
      var result =  foods.Where(food => food.Name.ToLower().Contains(term.ToLower())).Take(20).ToList();
     
      var json = Json(result, JsonRequestBehavior.AllowGet);

      return json;
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

      viewModel.DailyDietViewModels = viewModel.DailyDietViewModels.OrderBy(d => d.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).ToList();

      return viewModel;
    }
    
    
  }
}
