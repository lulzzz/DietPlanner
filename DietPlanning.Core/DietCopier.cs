using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core
{
  public static class DietCopier
  {
    public static Diet Copy(Diet diet)
    {
      var copy = new Diet();

      diet.DailyDiets.ForEach(dailyDiet => copy.DailyDiets.Add(CopyDailyDiet(dailyDiet)));

      return copy;
    }

    public static DailyDiet CopyDailyDiet(DailyDiet dailyDiet)
    {
      var copy = new DailyDiet();

      dailyDiet.Meals.ForEach(meal => copy.Meals.Add(CopyMeal(meal)));

      return copy;
    }

    public static Meal CopyMeal(Meal meal)
    {
      var copy = new Meal();

      meal.FoodPortions.ForEach(portion => copy.FoodPortions.Add(new FoodPortion(portion.Food, portion.Amount)));
      copy.Receipes.AddRange(meal.Receipes);

      return copy;
    }
  }
}
