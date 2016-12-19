using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Recipe
  {
    public List<FoodPortion> Ingredients;
    public NutritionValues NutritionValues;
    public int PreparationTimeInMinutes;
    public int Cost;
    public List<MealType> ApplicableMeals;
    public double NominalWeight;
    public MainCategory MainCategory;
    public SubCategory SubCategory;
    public int Id;

    public string Name;

    public Recipe()
    {
      Ingredients = new List<FoodPortion>();
      ApplicableMeals = new List<MealType>();
    }
  }
}
