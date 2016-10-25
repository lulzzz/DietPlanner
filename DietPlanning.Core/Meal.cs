using System.Collections.Generic;

namespace DietPlanning.Core
{
  public class Meal
  {
    public List<FoodPortion> FoodPortions;

    public Meal()
    {
      FoodPortions = new List<FoodPortion>();
    }
  }
}
