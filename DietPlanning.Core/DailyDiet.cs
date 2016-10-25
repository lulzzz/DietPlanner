using System.Collections.Generic;

namespace DietPlanning.Core
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