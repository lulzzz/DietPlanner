using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.GroupDiets
{
  public class GroupMeal
  {
    public MealType MealType;
    public List<RecipeGroupSplit> Recipes { get; set; }

    public GroupMeal()
    {
      Recipes = new List<RecipeGroupSplit>();
    }
  }
}