using System.Collections.Generic;

namespace DietPlanning.Web.Models
{
  public class MainCategoryPreferenceViewModel
  {
    public string Name { get; set; }
    public double Value { get; set; }
    public string Id { get; set; }
    public List<SubCategoryPreferenceViewModel> SubCategoryPreferences { get; set; }

    public MainCategoryPreferenceViewModel()
    {
      SubCategoryPreferences = new List<SubCategoryPreferenceViewModel>();
    }
  }
}