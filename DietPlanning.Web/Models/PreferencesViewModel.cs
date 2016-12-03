using System.Collections.Generic;

namespace DietPlanning.Web.Models
{
  public class PreferencesViewModel
  {
    public List<FoodNameViewModel> DislikedFoods { get; set; }
    public List<MainCategoryPreferenceViewModel> MainCategoryPreferences { get; set; }

    public PreferencesViewModel()
    {
      DislikedFoods = new List<FoodNameViewModel>();
      MainCategoryPreferences = new List<MainCategoryPreferenceViewModel>();
    }
  }
}