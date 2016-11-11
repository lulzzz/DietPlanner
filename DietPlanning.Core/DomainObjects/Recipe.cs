using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Recipe
  {
    //todo immutable

    public List<FoodPortion> Ingredients;
    public int PreparationTimeInMinutes;
    public int Cost;
    public RecipeGroup Group;
    public string Name;

    public Recipe()
    {
      Ingredients = new List<FoodPortion>();
    }
  }
}
