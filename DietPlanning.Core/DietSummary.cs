using System.Collections.Generic;

namespace DietPlanning.Core
{
  public class DietSummary
  {
    public double Calories { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }
    public double Proteins { get; set; }

    public List<double> CaloriesPerMeal { get; set; }

    public DietSummary()
    {
      CaloriesPerMeal = new List<double>();
    }
  }
}
