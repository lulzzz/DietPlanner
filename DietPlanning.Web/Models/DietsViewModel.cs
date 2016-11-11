using System.Collections.Generic;
using DietPlanning.Core;

namespace DietPlanning.Web.Models
{
  public class DietsViewModel
  {
    public DietSummary TargetDiet;
    public List<DietViewModel> Diets;

    public DietsViewModel()
    {
      Diets = new List<DietViewModel>();
    }
  }
}