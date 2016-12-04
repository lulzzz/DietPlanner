using System.Collections.Generic;
using DietPlanning.Core.GroupDiets;
using DietPlanning.NSGA;

namespace DietPlanning.Web.Models
{
  public class GroupDietViewModel
  {
    public List<Evaluation> Evaluations;
    public List<PersonDietViewModel> PersonDietViewModels;
    public GroupDiet DailyDiet;

    public GroupDietViewModel()
    {
      Evaluations = new List<Evaluation>();
      PersonDietViewModels = new List<PersonDietViewModel>();
    }
  }
}