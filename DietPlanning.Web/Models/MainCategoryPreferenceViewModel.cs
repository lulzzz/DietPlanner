using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Web.Models
{
  public class MainCategoryPreferenceViewModel
  {
    public string DisplayName { get; set; }
    public double Value { get; set; }
    public MainCategory MainCategory { get; set; }
    public List<SubCategoryPreferenceViewModel> SubCategoryPreferences { get; set; }

    public MainCategoryPreferenceViewModel()
    {
      SubCategoryPreferences = new List<SubCategoryPreferenceViewModel>();
    }
  }
}