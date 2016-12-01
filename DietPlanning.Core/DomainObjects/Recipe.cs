﻿using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Recipe
  {
    public List<FoodPortion> Ingredients;
    public NutritionValues NutritionValues;
    public int PreparationTimeInMinutes;
    public int Cost;
    public string MainCategory;
    public string SubCategory;

    public string Name;

    public Recipe()
    {
      Ingredients = new List<FoodPortion>();
    }
  }
}
