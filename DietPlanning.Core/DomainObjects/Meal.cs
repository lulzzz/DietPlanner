using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Meal
  {
    public List<FoodPortion> FoodPortions;
    public MealType MealType;
    public List<Recipe> Receipes { get; set; }

    public Meal()
    {
      Receipes = new List<Recipe>();
    }
  }
}
