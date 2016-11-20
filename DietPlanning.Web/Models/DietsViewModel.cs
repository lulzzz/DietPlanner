using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.Web.Models
{
  public class DietsViewModel
  {
    public DietRequirements DietRequirements;
    public List<DietViewModel> Diets;

    public DietsViewModel()
    {
      Diets = new List<DietViewModel>();
    }
  }
}