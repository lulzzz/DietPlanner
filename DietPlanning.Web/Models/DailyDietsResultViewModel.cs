using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.Web.Models
{
  public class DailyDietsResultViewModel
  {
    public DietRequirements DietRequirements;
    public List<DailyDietViewModel> DailyDietViewModels;

    public DailyDietsResultViewModel()
    {
      DailyDietViewModels = new List<DailyDietViewModel>();
    }
  }
}