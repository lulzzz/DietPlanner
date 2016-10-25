using FluentAssertions;
using NUnit.Framework;

namespace DietPlanning.Core.Tests
{
  [TestFixture]
  public class DietCopierTests
  {
    [Test]
    public void ShouldMakeFullCopyOfDiet()
    {
      var diet = new Diet();
      var dailyDiet = new DailyDiet();
      var meal = new Meal();
      var amount = 123;
      var food = new Food();
      var portion = new FoodPortion(food, amount);

      meal.FoodPortions.Add(portion);
      dailyDiet.Meals.Add(meal);
      diet.DailyDiets.Add(dailyDiet);

      DietCopier.Copy(diet).ShouldBeEquivalentTo(diet);
    }
  }
}
