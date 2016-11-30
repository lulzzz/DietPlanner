using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders.RandomData
{
  public class RandomRecipeProvider : IRecipeProvider
  {
    private readonly Random _random;
    private readonly int _size;
    private static List<Recipe> _recipes;
    private readonly IFoodsProvider _foodsProvider;

    public RandomRecipeProvider(Random random, int size, IFoodsProvider foodsProvider)
    {
      _random = random;
      _size = size;
      _foodsProvider = foodsProvider;
    }

    public List<Recipe> GetRecipes()
    {
      if (_recipes != null)
        return _recipes;

      RegenerateRecipes();

      return _recipes;
    }

    private void RegenerateRecipes()
    {
      _recipes = new List<Recipe>();
      var foodList = _foodsProvider.GetFoods();

      for (var i = 0; i < _size; i++)
      {
        var recipe = GetRandomRecipe(foodList);
        recipe.Name = "r" + i;
        _recipes.Add(recipe);
      }
    }

    private Recipe GetRandomRecipe(List<Food> foods)
    {
      var receipeGroups = Enum.GetValues(typeof(RecipeGroup));

      var recipe = new Recipe
      {
        Cost = _random.Next(5, 35),
        PreparationTimeInMinutes = _random.Next(1, 10)*10,
        Group = (RecipeGroup) receipeGroups.GetValue(_random.Next(receipeGroups.Length)),
        Ingredients = GetRandomFoods(foods)
      };

      return recipe;
    }

    private List<FoodPortion> GetRandomFoods(List<Food> foods)
    {
      var numberOfIngredients = _random.Next(2, 5);
      var foodPortions = new List<FoodPortion>();

      for (var i = 0; i < numberOfIngredients; i++)
      {
        Food food;
        do
        {
          food = foods[_random.Next(foods.Count)];
        } while (foodPortions.Any(portion => portion.Food == food));

        foodPortions.Add(new FoodPortion(food, _random.Next(20, 150)/numberOfIngredients));
      }

      return foodPortions;
    }

    private double CalculateCalories(Recipe recipe)
    {
      return recipe.Ingredients.Select(i => i.Food.Carbohydrates * i.Amount/100).Sum() * AtwaterFactors.Carbohydrates +
       recipe.Ingredients.Select(i => i.Food.Fat * i.Amount / 100).Sum() * AtwaterFactors.Fat +
       recipe.Ingredients.Select(i => i.Food.Proteins * i.Amount / 100).Sum()*AtwaterFactors.Proteins;
    }
  }
}
