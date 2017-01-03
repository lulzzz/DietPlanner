using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using DietPlanning.Core.DataProviders;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.FoodPreferences;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;
using DietPlanning.Web.Helpers;
using DietPlanning.Web.Models;
using DietPlanning.Web.Models.Builders;
using MultiAttributeDecisionMaking;
using RAdapter;

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

      RInvoker.Path = HostingEnvironment.MapPath(@"~/Content");
    }

    public ActionResult TopsisDiets()
    {
      var individuals = TempData.GetNsgaResult().Fronts.First().Select(i => (GroupDietIndividual) i).ToList();
      var solver = new Solver();

      var ordered = solver.TopsisSort(individuals, TempData.GetTopsisModel());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult AhpDiets()
    {
      var individuals = TempData.GetNsgaResult().Fronts.First().Select(i => (GroupDietIndividual)i).ToList();
      var solver = new Solver();

      var ordered = solver.AhpSort(individuals, TempData.GetAhpModel());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult ReferencePointDiets()
    {
      var individuals = TempData.GetNsgaResult().Fronts.First().Select(i => (GroupDietIndividual)i).ToList();
      var solver = new Solver();

      var ordered = solver.EuclideanSort(individuals, TempData.GetTopsisModel());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult Summary()
    {
      var dietsViewModel = TempData.GetGroupDietsResultViewModel();

      if (dietsViewModel == null)
      {
        return RedirectToAction("GerateSummary");
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

    [HttpGet]
    public ActionResult Ahp()
    {
      return View("Ahp", TempData.GetAhpModel());
    }

    [HttpPost]
    public ActionResult Ahp(AhpModel ahp)
    {
      TempData.SaveAhpModel(ahp);

      return View("Topsis");
    }

    [HttpGet]
    public ActionResult Topsis()
    {
      return View("Topsis", TempData.GetTopsisModel());
    }

    [HttpPost]
    public ActionResult Topsis(WeightsModel weights)
    {
      TempData.SaveTopsisModel(weights);

      return View("Ahp");
    }

    public ActionResult GerateGroupDiets()
    {
      var dietRequirements = _requirementsProvider.GetRequirements(TempData.GetPersonalData(), 5);
      var personalData = TempData.GetPersonalDataList();
      personalData.ForEach(pd => pd.Requirements = _requirementsProvider.GetRequirements(pd, 5));

      var recipes = _recipeProvider.GetRecipes();
      var dietPreferences = TempData.GetDietPreferences();
      personalData.ForEach(pd => pd.Preferences = dietPreferences);
      TempData.SavePersonalDataList(personalData);

      var nsgaSolver = _nsgaSolverFactory.GetGroupDietSolver(recipes, personalData, TempData.GetSettings().NsgaConfiguration);

      var nsgaResult = nsgaSolver.Solve();
      TempData.SaveLog(nsgaResult.Log);
      TempData.SaveNsgaResult(nsgaResult);
      var viewModelBuilder = new GroupDietViewModelBuilder();

      var dietsViewModel = viewModelBuilder.Build(nsgaResult, personalData);
      TempData.SaveGroupDietsResultViewModel(dietsViewModel);

      return RedirectToAction("GroupDiets");
    }

    public ActionResult GerateSummary()
    {
      var dietRequirements = _requirementsProvider.GetRequirements(TempData.GetPersonalData(), 5);
      var personalData = TempData.GetPersonalDataList();
      personalData.ForEach(pd => pd.Requirements = _requirementsProvider.GetRequirements(pd, 5));

      var recipes = _recipeProvider.GetRecipes();
      var dietPreferences = TempData.GetDietPreferences();
      personalData.ForEach(pd => pd.Preferences = dietPreferences);
      TempData.SavePersonalDataList(personalData);
      var nsgaSolver = _nsgaSolverFactory.GetGroupDietSolver(recipes, personalData, TempData.GetSettings().NsgaConfiguration);

      var nsgaResult = nsgaSolver.Solve();
      TempData.SaveLog(nsgaResult.Log);
      TempData.SaveNsgaResult(nsgaResult);

      var viewModelBuilder = new GroupDietViewModelBuilder();

      var dietsViewModel = viewModelBuilder.Build(nsgaResult, personalData);
      TempData.SaveGroupDietsResultViewModel(dietsViewModel);

      return RedirectToAction("Summary");
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
      //var foods = new FoodDatabaseProvider().GetFoods().ToList();
      //var result =  foods.Where(food => food.Name.ToLower().Contains(term.ToLower())).Take(20).ToList();
     
      //var json = Json(result, JsonRequestBehavior.AllowGet);

      //return json;

      return new JsonResult();
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
