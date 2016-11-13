using System.Collections.Generic;
using DietPlanning.Core;

namespace DietPlanning.Web.Models
{
  public class DailyDietsResultViewModel
  {
    public DietSummary TargetDiet;
    public List<DailyDietViewModel> DailyDietViewModels;

    public DailyDietsResultViewModel()
    {
      DailyDietViewModels = new List<DailyDietViewModel>();
    }
  }
}