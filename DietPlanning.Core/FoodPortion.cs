namespace DietPlanning.Core
{
  public class FoodPortion
  {
    public Food Food;
    public int Amount;

    public FoodPortion(Food food, int amount)
    {
      Food = food;
      Amount = amount;
    }
  }
}
