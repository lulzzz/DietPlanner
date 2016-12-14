using System;
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
    private Dictionary<MealType, List<Recipe>> _recipesForMealType;

    public GroupDietPopulationInitializer(Random random, List<Recipe> recipes, int numberOfMealsPerDay, List<PersonalData> personalData, GroupDietAnalyzer dietAnalyzer)
    {
      _random = random;
      _recipes = recipes;
      _numberOfMealsPerDay = numberOfMealsPerDay;
      _personalData = personalData;
      _dietAnalyzer = dietAnalyzer;
      InitializeRecipesForMealType();
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
        var meal = new GroupMeal { MealType = (MealType)j };
        diet.Meals.Add(meal);
      }

      List<int> idsWithDeficiency;

      while ( (idsWithDeficiency = _personalData.Where(p => p.Requirements.CaloriesAllowedRange.Lower > _dietAnalyzer.SummarizeForPerson(diet, p.Id).NutritionValues.Calories).Select(pd => pd.Id).ToList()).Any())
      {
        var meal = diet.Meals.GetRandomItem();
        var includedRecipes = diet.Meals.SelectMany(m => m.Recipes).Select(r => r.Recipe).Distinct().ToList();
        var split = GetRanodmNotInvlolvedRecipe(includedRecipes, meal.MealType);

        split.Adjustments.RemoveAll(a => !idsWithDeficiency.Contains(a.PersonId));
        meal.Recipes.Add(split);
      }

      return diet;
    }

    private RecipeGroupSplit GetRanodmNotInvlolvedRecipe(List<Recipe> includedRecipes, MealType mealType)
    {
      var recipeGroupSplit = new RecipeGroupSplit(_personalData.Count);

      do
      {
        recipeGroupSplit.Recipe = _recipesForMealType[mealType].GetRandomItem();
      } while (includedRecipes.Contains(recipeGroupSplit.Recipe));

      return recipeGroupSplit;
    }

    private void InitializeRecipesForMealType()
    {
      _recipesForMealType = new Dictionary<MealType, List<Recipe>>();

      foreach (MealType mealType in Enum.GetValues(typeof(MealType)))
      {
        _recipesForMealType.Add(mealType, _recipes.Where(r => r.ApplicableMeals.Contains(mealType)).ToList());
      }
    }
  }
}
