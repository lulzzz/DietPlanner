using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.NSGA;

namespace DietPlanning.Web.Models
{
  public class DietViewModel
  {
    public List<DailyDietViewModel> DailyDiets;
    public List<Evaluation> Evaluations;
    public DietSummary AverageDietSummary;

    public DietViewModel()
    {
      Evaluations = new List<Evaluation>();
    }
  }
}