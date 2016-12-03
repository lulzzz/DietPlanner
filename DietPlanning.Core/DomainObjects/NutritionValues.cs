namespace DietPlanning.Core.DomainObjects
{
  public class NutritionValues
  {
    public double Proteins { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }
    public double Calories { get; set; }

    public static NutritionValues operator +(NutritionValues left, NutritionValues right)
    {
      return new NutritionValues
      {
        Proteins = left.Proteins + right.Proteins,
        Fat = left.Fat + right.Fat,
        Carbohydrates = left.Carbohydrates + right.Carbohydrates,
        Calories = left.Calories + right.Calories
      };
    }

    public static NutritionValues operator *(NutritionValues values, double multiplier)
    {
      return new NutritionValues
      {
        Proteins = values.Proteins*multiplier,
        Fat = values.Fat*multiplier,
        Carbohydrates = values.Carbohydrates*multiplier,
        Calories = values.Calories*multiplier,
      };
    }
  }
}
