using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core
{
  public class DietSummary
  {
    public NutritionValues NutritionValues { get; set; }
   
    public List<double> CaloriesPerMeal { get; set; }

    public DietSummary()
    {
      CaloriesPerMeal = new List<double>();
      NutritionValues = new NutritionValues();
    }
  }
}
