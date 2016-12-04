using System.Collections.Generic;

namespace DietPlanning.Core.GroupDiets
{
  public class GroupDiet
  {
    public List<GroupMeal> Meals;

    public GroupDiet()
    {
      Meals = new List<GroupMeal>();
    }
  }
}
