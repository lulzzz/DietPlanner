using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;
using DietPlanning.Web.Models.Builders;

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

    public ActionResult GroupDiets()
    {
      var dietsViewModel = TempData.GetGroupDietsResultViewModel();

      if (dietsViewModel == null)
      {
        return RedirectToAction("GerateGroupDiets");
      }

      return View(dietsViewModel);
    }

    public ActionResult GenerateDiets()
    {
      var dietRequirements = _requirementsProvider.GetRequirements(TempData.GetPersonalData(), 5);
      
      var recipes = _recipeProvider.GetRecipes();
      var dietPreferences = TempData.GetDietPreferences();
      var nsgaSolver = _nsgaSolverFactory.GetDailyDietsSolver(TempData.GetSettings().NsgaConfiguration, recipes, dietRequirements, dietPreferences);

      var nsgaResult = nsgaSolver.Solve();
      TempData.SaveLog(nsgaResult.Log);

      var dietsViewModel = CreateDailyDietsResultViewModel(nsgaResult.Fronts, dietRequirements);
      TempData.SaveDailyDietsResultViewModel(dietsViewModel);

      return RedirectToAction("ShowDays");
    }

    public ActionResult GerateGroupDiets()
    {
      var dietRequirements = _requirementsProvider.GetRequirements(TempData.GetPersonalData(), 5);
      var personalData = TempData.GetPersonalDataList();
      personalData.ForEach(pd => pd.Requirements = _requirementsProvider.GetRequirements(pd, 5));


      var recipes = _recipeProvider.GetRecipes();
      var dietPreferences = TempData.GetDietPreferences();
      //  var nsgaSolver = _nsgaSolverFactory.GetDailyDietsSolver(TempData.GetSettings().NsgaConfiguration, recipes, dietRequirements, dietPreferences);
      var nsgaSolver = _nsgaSolverFactory.GetGroupDietSolver(recipes, personalData, TempData.GetSettings().NsgaConfiguration);

      var nsgaResult = nsgaSolver.Solve();
      TempData.SaveLog(nsgaResult.Log);

      var viewModelBuilder = new GroupDietViewModelBuilder();

      var dietsViewModel = viewModelBuilder.Build(nsgaResult, personalData);
      TempData.SaveGroupDietsResultViewModel(dietsViewModel);

      return RedirectToAction("GroupDiets");
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
      var preferences = TempData.GetPreferencesViewModel() ?? InitializePreferencesViewModel();

      TempData.SavePreferencesViewModel(preferences);

      return View(preferences);
    }

    [HttpPost]
    public ActionResult Preferences(PreferencesViewModel preferences)
    {
      UpdatePreferences(preferences);

      return View(TempData.GetPreferencesViewModel());
    }

    [HttpGet]
    public JsonResult GetFoods(string term)
    {
      var foods = new FoodDatabaseProvider().GetFoods().ToList();
      var result =  foods.Where(food => food.Name.ToLower().Contains(term.ToLower())).Take(20).ToList();
     
      var json = Json(result, JsonRequestBehavior.AllowGet);

      return json;
    }

    private void UpdatePreferences(PreferencesViewModel preferencesViewModel)
    {
      var preferences = TempData.GetDietPreferences();
      preferences.CategoryPreferences.Clear();

      var oldPreferencesViewModel = TempData.GetPreferencesViewModel();

      var oldMains = oldPreferencesViewModel.MainCategoryPreferences.ToList();
      var oldSubs = oldPreferencesViewModel.MainCategoryPreferences.SelectMany(m => m.SubCategoryPreferences).ToList();
      var mewMains = preferencesViewModel.MainCategoryPreferences.ToList();
      var newSubs = preferencesViewModel.MainCategoryPreferences.SelectMany(m => m.SubCategoryPreferences).ToList();

      foreach (var oldMain in oldMains)
      {
        oldMain.Value = mewMains.Single(m => m.Id == oldMain.Id).Value;
        if (oldMain.Value != 0.0)
        {
          preferences.CategoryPreferences.Add(new CategoryPreference
          {
            CategoryLevel = CategoryLevel.MainCategory,
            Name = oldMain.Name,
            Preference = oldMain.Value
          });
        }
      }

      foreach (var oldSub in oldSubs)
      {
        oldSub.Value = newSubs.Single(m => m.Id == oldSub.Id).Value;
        if (oldSub.Value != 0.0)
        {
          preferences.CategoryPreferences.Add(new CategoryPreference
          {
            CategoryLevel = CategoryLevel.SubCategory,
            Name = oldSub.Name,
            Preference = oldSub.Value
          });
        }
      }

      TempData.SaveDietPreferences(preferences);
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

    private PreferencesViewModel InitializePreferencesViewModel()
    {
      var recipes = _recipeProvider.GetRecipes();
      var mainCategories = recipes.Select(recipe => recipe.MainCategory).Distinct();
      var preferencesViewModel = new PreferencesViewModel();
      var preferenceIndex = 0;

      foreach (var mainCategory in mainCategories)
      {
        var mainCategoryPreferenceViewModel = new MainCategoryPreferenceViewModel
        {
          Name = mainCategory,
          Value = 0.0,
          Id = $"preference_{++preferenceIndex}"
        };
        var subCategories =
          recipes.Where(r => r.MainCategory == mainCategory && r.MainCategory != r.SubCategory)
          .Select(r => r.SubCategory).Distinct().Where(subCategory => subCategory != mainCategory);

        foreach (var subCategory in subCategories)
        {
          var subCategoryPreferenceViewModel = new SubCategoryPreferenceViewModel
          {
            Name = subCategory,
            Value = 0,
            Id = $"preference_{++preferenceIndex}"
          };
          mainCategoryPreferenceViewModel.SubCategoryPreferences.Add(subCategoryPreferenceViewModel);
        }

        preferencesViewModel.MainCategoryPreferences.Add(mainCategoryPreferenceViewModel);
      }

      return preferencesViewModel;
    }
  }
}
