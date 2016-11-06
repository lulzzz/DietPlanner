using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class DailyDiet
  {
    public List<Meal> Meals;

    public DailyDiet()
    {
      Meals = new List<Meal>();
    }
  }
}