using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;
using DietPlanning.NSGA.GroupDietsImplementation;
using DietPlanning.Web.Models.Builders;
using FluentAssertions;
using NUnit.Framework;

namespace DietPlanning.NSGA.Tests
{
  class GroupEvaluatorUnitTests
  {
    [Test]
    public void T1()
    {
      var groupDiet = GetDiet();
      var pd = new List<PersonalData> {GetPersonalData(0), GetPersonalData(1) };
      var individual = new GroupDietIndividual(groupDiet);

      var ev = new GroupDietEvaluator(pd, new GroupDietAnalyzer());

      ev.Evaluate(individual);

      var builder = new GroupDietViewModelBuilder();
      var nsgRes = new NsgaResult();

      nsgRes.Fronts.Add(new List<Individual> {individual});

      var vm = builder.Build(nsgRes, pd);

      vm.GroupDiets.First().Evaluations.First().Score.Should().Be(250);
    }

    private GroupDiet GetDiet()
    {
      var groupDiet = new GroupDiet();
      groupDiet.Meals.Add(new GroupMeal());
      groupDiet.Meals.Add(new GroupMeal());
      groupDiet.Meals.Add(new GroupMeal());
      groupDiet.Meals.Add(new GroupMeal());
      groupDiet.Meals.Add(new GroupMeal());

      var split = new RecipeGroupSplit(2);
      split.Adjustments.RemoveAt(1);
      split.Recipe = new Recipe
      {
        NominalWeight = 100,
        NutritionValues = new NutritionValues
        {
          Calories = 250,
          Proteins = 250
        }
      };
      groupDiet.Meals[2].Recipes.Add(split);

      return groupDiet;
    }

    private PersonalData GetPersonalData(int id)
    {
      var personalData = new PersonalData();

      personalData.Id = id;
      personalData.Requirements = new DietRequirements
      {
        CaloriesAllowedRange = new Range {Lower = 100, Upper = 200},
        FatRange = new Range {Lower = 0, Upper = 1000},
        ProteinRange = new Range {Lower = 500, Upper = 6000},
        CarbohydratesRange = new Range {Lower = 0, Upper = 1000},
        MealCaloriesSplit = new List<Range>
        {
          new Range {Lower = 0, Upper = 1000},
          new Range {Lower = 0, Upper = 1000},
          new Range {Lower = 0, Upper = 1000},
          new Range {Lower = 0, Upper = 1000},
          new Range {Lower = 0, Upper = 1000},
        }
      };

      return personalData;
    }
  }
}
