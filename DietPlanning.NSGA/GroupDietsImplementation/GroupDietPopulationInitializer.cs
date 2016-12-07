﻿using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.GroupDiets;
using DietPlanning.Core.NutritionRequirements;
using Tools;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  class GroupDietPopulationInitializer : IPopulationInitializer
  {
    private readonly Random _random;
    private readonly List<Recipe> _recipes;
    private readonly int _numberOfMealsPerDay;
    private readonly List<PersonalData> _personalData;
    private readonly GroupDietAnalyzer _dietAnalyzer;

    public GroupDietPopulationInitializer(Random random, List<Recipe> recipes, int numberOfMealsPerDay, List<PersonalData> personalData, GroupDietAnalyzer dietAnalyzer)
    {
      _random = random;
      _recipes = recipes;
      _numberOfMealsPerDay = numberOfMealsPerDay;
      _personalData = personalData;
      _dietAnalyzer = dietAnalyzer;
    }

    public List<Individual> InitializePopulation(int populationSize)
    {
      var population = new List<Individual>();

      for (var i = 0; i < populationSize; i++)
      {
        population.Add(new GroupDietIndividual(CreateRandomDailyDiet()));
      }

      return population;
    }

    private GroupDiet CreateRandomDailyDiet()
    {
      var diet = new GroupDiet();

      for (var j = 0; j < _numberOfMealsPerDay; j++)
      {
        var meal = CreateRandomMeal();
        meal.MealType = (MealType)j;
        diet.Meals.Add(meal);
      }

      var topCaloriesRange = _personalData.Select(p => p.Requirements.CaloriesAllowedRange.Upper).Max();

      //var ranges = _personalData.Select(pd => pd.Requirements.CaloriesAllowedRange).ToList();

      //var oldDistance = GetTotalCaloriesDistance(diet, ranges);
      //var newDistance = oldDistance;

      //while (Math.Abs(newDistance) <= Math.Abs(oldDistance))
      //{
      //  AddRandomReceipeSplit(diet.Meals.GetRandomItem());

      //  oldDistance = newDistance;
      //  newDistance = GetTotalCaloriesDistance(diet, ranges);
      //}

      //while (_dietAnalyzer.SummarizeForPerson(diet, _personalData.First().Id).NutritionValues.Calories < topCaloriesRange)
      //{
      //  AddRandomReceipeSplit(diet.Meals.GetRandomItem());
      //}

      var ranges = _personalData.Select(pd => pd.Requirements.CaloriesAllowedRange).ToList();

      List<int> idsWithDeficiency;
      var idsWithToMuchCalories = _personalData.Where(p => p.Requirements.CaloriesAllowedRange.Upper < _dietAnalyzer.SummarizeForPerson(diet, p.Id).NutritionValues.Calories).Select(pd => pd.Id).ToList();
      while (
      (idsWithDeficiency =
        _personalData.Where(p => p.Requirements.CaloriesAllowedRange.Lower > _dietAnalyzer.SummarizeForPerson(diet, p.Id).NutritionValues.Calories).Select(pd => pd.Id).ToList()).Any())
      {
        var includedRecipes = diet.Meals.SelectMany(m => m.Recipes).Select(r => r.Recipe).Distinct().ToList();
        var split = GetRanodmNotInvlolvedRecipe(includedRecipes);
        split.Adjustments.RemoveAll(a => !idsWithDeficiency.Contains(a.PersonId));
        diet.Meals.GetRandomItem().Recipes.Add(split);
      }

      return diet;
    }

    private double GetTotalCaloriesDistance(GroupDiet diet, List<Range> caloriesRanges)
    {
     
      var summaries = _personalData.Select(pd => pd.Id).ToList().Select(id => _dietAnalyzer.SummarizeForPerson(diet, id)).ToList();

      return caloriesRanges.Select((range, i) => range.GetDistanceToRange(summaries[i].NutritionValues.Calories)).Sum();
    }

    private GroupMeal CreateRandomMeal()
    {
      var meal = new GroupMeal();
      var numberOfRecipes = _random.Next(1, 4);

      for (var k = 0; k < numberOfRecipes; k++)
      {
        AddRandomReceipeSplit(meal);
      }

      return meal;
    }

    private void AddRandomReceipeSplit(GroupMeal meal)
    {
      Recipe recipe;

      do
      {
        recipe = _recipes.GetRandomItem();
      } while (meal.Recipes.Any(r => r.Recipe == recipe));

      var recipeSlpit = new RecipeGroupSplit(_personalData.Count) {Recipe = recipe};

      meal.Recipes.Add(recipeSlpit);
    }

    private RecipeGroupSplit GetRanodmNotInvlolvedRecipe(List<Recipe> includedRecipes)
    {
      var recipeGroupSplit = new RecipeGroupSplit(_personalData.Count);

      do
      {
        recipeGroupSplit.Recipe = _recipes.GetRandomItem();
      } while (includedRecipes.Contains(recipeGroupSplit.Recipe));

      return recipeGroupSplit;
    }

  }
}
