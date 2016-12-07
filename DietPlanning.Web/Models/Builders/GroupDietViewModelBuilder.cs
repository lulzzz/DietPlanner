using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;

namespace DietPlanning.Web.Models.Builders
{
  public class GroupDietViewModelBuilder
  {
    public GroupDietsResultViewModel Build(NsgaResult result, List<PersonalData> personalDataList)
    {
      var groupDietsResultViewModel = new GroupDietsResultViewModel();

      groupDietsResultViewModel.PersonalDatas = personalDataList;
      groupDietsResultViewModel.GroupDiets = CreateGroupDietsViewModel(result, personalDataList);
      groupDietsResultViewModel.GroupDiets = groupDietsResultViewModel.GroupDiets.OrderBy(d => d.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).ToList();
      return groupDietsResultViewModel;
    }

    private List<GroupDietViewModel> CreateGroupDietsViewModel(NsgaResult result, List<PersonalData> personalDataList)
    {
      var models = new List<GroupDietViewModel>();

      var individuals = result.Fronts.SelectMany(f => f.Select(i => i as GroupDietIndividual));

      foreach (var individual in individuals)
      {
        models.Add(CreateGroupDietViewModel(individual, personalDataList));
      }

      return models;
    }

    private GroupDietViewModel CreateGroupDietViewModel(GroupDietIndividual individual, List<PersonalData> personalDataList)
    {
      var groupDietViewModel = new GroupDietViewModel();
      var dietAnalizer = new GroupDietAnalyzer();

      groupDietViewModel.Evaluations = individual.Evaluations;

      foreach (var personalData in personalDataList)
      {
        var personDiet = new PersonDietViewModel();

        personDiet.DietSummary = dietAnalizer.SummarizeForPerson(individual.GroupDiet, personalData.Id);
        personDiet.Meals = GetMealsViewModel(personDiet.DietSummary, individual, personalData);
        groupDietViewModel.PersonDietViewModels.Add(personDiet);
      }

      return groupDietViewModel;
    }

    private List<MealViewModel> GetMealsViewModel(DietSummary dietSummary, GroupDietIndividual individual, PersonalData personalData)
    {
      var mealViewModels = new List<MealViewModel>();

      foreach (var meal in individual.GroupDiet.Meals)
      {
        var mealVm = new MealViewModel();
        var recipes = meal.GetRecipesForPerson(personalData.Id);
        var mealindex = individual.GroupDiet.Meals.IndexOf(meal);
        recipes.ForEach(r => mealVm.Recipes.Add(new RecipeViewModel {Amount = r.Item1 * r.Item2.NominalWeight, Name = r.Item2.Name}));
        mealVm.Calories = (int)dietSummary.CaloriesPerMeal[mealindex];
        mealVm.IsInRange = personalData.Requirements.MealCaloriesSplit[mealindex].IsInRange(mealVm.Calories);
        mealVm.DistanceToRange = (int)personalData.Requirements.MealCaloriesSplit[mealindex].GetDistanceToRange(mealVm.Calories);

        mealViewModels.Add(mealVm);
      }

      return mealViewModels;
    }
  }
}