using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Meal
  {
    public List<FoodPortion> FoodPortions;
    public List<Recipe> Receipes { get; set; }

    public Meal()
    {
      FoodPortions = new List<FoodPortion>(); //todo: remove
      Receipes = new List<Recipe>();
    }
  }
}
