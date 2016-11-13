using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;
using DietPlanning.NSGA;

namespace DietPlanning.Web.Models
{
  public class DailyDietViewModel
  {
    public List<Evaluation> Evaluations;
    public DietSummary DietSummary;
    public DailyDiet DailyDiet;

    public DailyDietViewModel()
    {
      Evaluations = new List<Evaluation>();
    }
  }
}