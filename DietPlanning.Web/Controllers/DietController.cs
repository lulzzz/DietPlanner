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

      var ordered = solver.TopsisSort(individuals, TempData.GetPersonalDataList().Select(p => TempData.GetPreferencePointModel(p.Id)).ToList());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult AhpDiets()
    {
      var individuals = TempData.GetNsgaResult().Fronts.First().Select(i => (GroupDietIndividual)i).ToList();
      var solver = new Solver();

      var ordered = solver.AhpSort(individuals, TempData.GetPersonalDataList().Select(p => TempData.GetAhpModel(p.Id)).ToList());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult ReferencePointDiets()
    {
      var individuals = TempData.GetNsgaResult().Fronts.First().Select(i => (GroupDietIndividual)i).ToList();
      var solver = new Solver();

      var ordered = solver.EuclideanSort(individuals, TempData.GetPersonalDataList().Select(p => TempData.GetPreferencePointModel(p.Id)).ToList());

      var model = ordered.Select(i => new GroupDietViewModelBuilder().CreateGroupDietViewModel(i, TempData.GetPersonalDataList())).Take(5).ToList();

      return PartialView("DietsPartial", model);
    }

    public ActionResult Summary()
    {
      var dietsViewModel = TempData.GetGroupDietsResultViewModel();

      if (dietsViewModel == null)
      {
        return RedirectToAction("GenerateSummary");
      }

      return View(dietsViewModel);
    }

    public ActionResult GenerateSummary()
    {
      var personalData = TempData.GetPersonalDataList();
      personalData.ForEach(pd => pd.Requirements = _requirementsProvider.GetRequirements(pd, 5));
      var bannedSubCategories = GetBannedSubCategories();
      var bannedMainCategories = GetBannedMainCategories();

      var recipes = _recipeProvider.GetRecipes().Where(r => !bannedMainCategories.Contains(r.MainCategory) && !bannedSubCategories.Contains(r.SubCategory)).ToList();

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
    public JsonResult GetFoods(string term)
    {
      //var foods = new FoodDatabaseProvider().GetFoods().ToList();
      //var result =  foods.Where(food => food.Name.ToLower().Contains(term.ToLower())).Take(20).ToList();
     
      //var json = Json(result, JsonRequestBehavior.AllowGet);

      //return json;

      return new JsonResult();
    }

    private List<MainCategory> GetBannedMainCategories()
    {
      var preferencesModels = TempData.GetPersonalDataList().Select(pd => TempData.GetPreferencesViewModel(pd.Id));

      var bannedCategories = preferencesModels.SelectMany(p => p.MainCategoryPreferences.Where(cp => cp.Value <= 0.5));

      return bannedCategories.Select(cat => cat.MainCategory).Distinct().ToList();
    }

    private List<SubCategory> GetBannedSubCategories()
    {
      var preferencesModels = TempData.GetPersonalDataList().Select(pd => TempData.GetPreferencesViewModel(pd.Id));

      var bannedCategories = preferencesModels.SelectMany(p => p.MainCategoryPreferences).SelectMany(p => p.SubCategoryPreferences).Where(cp => cp.Value <= 0.5);

      return bannedCategories.Select(cat => cat.SubCategory).Distinct().ToList();
    }
  }
}
