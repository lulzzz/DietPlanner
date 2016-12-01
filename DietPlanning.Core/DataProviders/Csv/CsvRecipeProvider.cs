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

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var values = line.Split(';');
        var recipe = new Recipe
        {
          Name = values[0],

          NutritionValues = new NutritionValues()
          {
            Calories = ParseValue(values[4]),
            Carbohydrates = ParseValue(values[7]),
            Proteins = ParseValue(values[6]),
            Fat = ParseValue(values[5]),
          },

          MainCategory = values[2],
          SubCategory = values[1],

          Cost = _random.Next(5, 35),
          PreparationTimeInMinutes = _random.Next(1, 10) * 10
        };

        recipes.Add(recipe);
      }
      
      return recipes;
    }

    private static double ParseValue(string value)
    {
      return double.Parse(value);
    }

  }
}
