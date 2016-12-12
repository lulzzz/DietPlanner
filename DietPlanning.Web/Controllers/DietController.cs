using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DietPlanning.Core;
using DietPlanning.Core.DataProviders;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.DayImplementation;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;
using DietPlanning.Web.Models.Builders;
using WebGrease.Css.Extensions;

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

      var oldPreferences = TempData.GetPreferencesViewModel();
      var oldMainPrefs = oldPreferences.MainCategoryPreferences;
      var oldSubPrefs = oldPreferences.MainCategoryPreferences.SelectMany(m => m.SubCategoryPreferences).ToList();

      foreach (var mainCategoryPreference in preferencesViewModel.MainCategoryPreferences)
      {
        List<SubCategoryPreferenceViewModel> subCategories;

        if (Math.Abs(mainCategoryPreference.Value - 1.0) > 0.01)
        {
          oldMainPrefs.Single(m => m.MainCategory == mainCategoryPreference.MainCategory).Value = mainCategoryPreference.Value;
          subCategories = mainCategoryPreference.SubCategoryPreferences;
        }
        else
        {
          subCategories = mainCategoryPreference.SubCategoryPreferences.Where(sub => Math.Abs(sub.Value - 1.0) > 0.01).ToList();
        }

        foreach (var subCategory in subCategories)
        {
          oldSubPrefs.Single(s => s.SubCategory ==  subCategory.SubCategory).Value = subCategory.Value;

          preferences.CategoryPreferences.Add(new CategoryPreference
          {
            Preference = subCategory.Value*mainCategoryPreference.Value,
            SubCategory = subCategory.SubCategory
          });
        }
      }

      TempData.SavePreferencesViewModel(oldPreferences);
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
      var preferencesViewModel = new PreferencesViewModel();

      foreach (MainCategory category in Enum.GetValues(typeof(MainCategory)))
      {
        var mainCategoryPreference = new MainCategoryPreferenceViewModel
        {
          MainCategory = category,
          DisplayName = category.ToString(),
          Value = 1,
          SubCategoryPreferences = CreateSubCategoryPreferenceViewModels(category)
        };
        
        preferencesViewModel.MainCategoryPreferences.Add(mainCategoryPreference);
      }
      
      return preferencesViewModel;
    }

    private List<SubCategoryPreferenceViewModel> CreateSubCategoryPreferenceViewModels(MainCategory mainCategory)
    {
      var subCategories = GroupsMapping.SubToMainCategoryMapping.Where(k => k.Value == mainCategory).Select(k => k.Key);

      return subCategories.Select(subCategory => new SubCategoryPreferenceViewModel
      {
        Value = 1.0, DisplayName = subCategory.ToString(), SubCategory = subCategory
      }).ToList();
    }
  }
}
