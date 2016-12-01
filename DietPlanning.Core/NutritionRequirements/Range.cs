namespace DietPlanning.Core.NutritionRequirements
{
  public class Range
  {
    public double Lower { get; set; }
    public double Upper { get; set; }


    public bool IsInRange(double value)
    {
      return value > Lower && value < Upper;
    }

    public double GetDistanceToRange(double value)
    {
      if (value < Lower)
      {
        return Lower - value;
      }
      if (value > Upper)
      {
        return Upper - value;
      }

      return 0.0;
    }
  }
}