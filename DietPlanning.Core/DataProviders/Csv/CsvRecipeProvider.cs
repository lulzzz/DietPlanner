using System;
using System.Collections.Generic;
using System.IO;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders.Csv
{
  public class CsvRecipeProvider : IRecipeProvider
  {
    private readonly Random _random;
    private readonly string _path;
    private List<Recipe> _recipes;

    public CsvRecipeProvider(Random random, string path)
    {
      _random = random;
      _path = path;
    }

    public List<Recipe> GetRecipes()
    {
      return _recipes ?? (_recipes = ReadRecipes());
    }

    private List<Recipe> ReadRecipes()
    {
      var reader = new StreamReader(File.OpenRead(_path));
      var recipes = new List<Recipe>();

      reader.ReadLine(); //skip headers
      reader.ReadLine(); //skip headers

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var values = line.Split(';');

        if (string.IsNullOrEmpty(values[(int)CsvField.CanBeUsed]))
          continue;

        var recipe = new Recipe
        {
          Name = values[(int)CsvField.Name],

          NutritionValues = new NutritionValues()
          {
            Calories = ParseValue(values[(int)CsvField.Calories]),
            Carbohydrates = ParseValue(values[(int)CsvField.Carbs]),
            Proteins = ParseValue(values[(int)CsvField.Proteins]),
            Fat = ParseValue(values[(int)CsvField.Fats]),
          },
          NominalWeight = ParseValue(values[(int)CsvField.NominalWeight]),
          MainCategory = FoodGroupsMapper.MainCategoryCsvMapper[values[(int)CsvField.MainCategory]],
          SubCategory = FoodGroupsMapper.SubCategoryCsvMapper[values[(int)CsvField.SubCategory]],
          Cost = (int)ParseValue(values[(int)CsvField.Cost]),
          PreparationTimeInMinutes = (int)ParseValue(values[(int)CsvField.PreparationTime]),
          Id = (int)ParseValue(values[(int)CsvField.Id]),
        };

        if (!string.IsNullOrEmpty(values[(int)CsvField.ApplicableForMeal1]))
          recipe.ApplicableMeals.Add(MealType.Breakfast);

        if (!string.IsNullOrEmpty(values[(int)CsvField.ApplicableForMeal2]))
          recipe.ApplicableMeals.Add(MealType.SecondBreakfast);

        if (!string.IsNullOrEmpty(values[(int)CsvField.ApplicableForMeal3]))
          recipe.ApplicableMeals.Add(MealType.Dinner);

        if (!string.IsNullOrEmpty(values[(int)CsvField.ApplicableForMeal4]))
          recipe.ApplicableMeals.Add(MealType.Lunch);

        if (!string.IsNullOrEmpty(values[(int)CsvField.ApplicableForMeal5]))
          recipe.ApplicableMeals.Add(MealType.Supper);


        recipes.Add(recipe);
      }

      return recipes;
    }

    private static double ParseValue(string value)
    {
      return double.Parse(value);
    }

  }

  internal enum CsvField
  {
    Name = 0,
    NominalWeight = 1,
    ApplicableForMeal1 = 2,
    ApplicableForMeal2 = 3,
    ApplicableForMeal3 = 4,
    ApplicableForMeal4 = 5,
    ApplicableForMeal5 = 6,
    CanBeUsed = 8,
    SubCategory = 9,
    MainCategory = 10,
    Type = 11,
    Calories = 12,
    Fats = 13,
    Proteins = 14,
    Carbs = 15,
    Cost = 16,
    PreparationTime = 17,
    Id = 18
  }
}
