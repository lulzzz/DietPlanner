using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Recipe
  {
    public List<FoodPortion> Ingredients;
    public int PreparationTimeInMinutes;
    public int Cost;
    public RecipeGroup Group;

    public Recipe()
    {
      Ingredients = new List<FoodPortion>();
    }
  }
}
