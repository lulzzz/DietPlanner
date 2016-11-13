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
    private readonly List<Food> _foods;

    public RandomRecipeProvider(Random random, int size, List<Food> foods)
    {
      _random = random;
      _size = size;
      _foods = foods;
    }

    public List<Recipe> GetRecipes()
    {
      var recipes = new List<Recipe>();

      for (var i = 0; i < _size; i++)
      {
        var recipe = GetRandomRecipe(_foods);
        recipe.Name = "r" + i;
        recipes.Add(recipe);
      }

      return recipes;
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
      var numberOfIngredients = _random.Next(2, 6);
      var foodPortions = new List<FoodPortion>();

      for (var i = 0; i < numberOfIngredients; i++)
      {
        Food food;
        do
        {
          food = foods[_random.Next(foods.Count)];
        } while (foodPortions.Any(portion => portion.Food == food));

        foodPortions.Add(new FoodPortion(food, _random.Next(10, 150)));
      }

      return foodPortions;
    }
  }
}
