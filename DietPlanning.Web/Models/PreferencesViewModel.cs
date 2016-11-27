using System.Collections.Generic;

namespace DietPlanning.Web.Models
{
  public class PreferencesViewModel
  {
    public List<FoodGroupPreference> FoodGroupPreferences { get; set; }
    public List<RecipeGroupPreference> RecipeGroupPreferences { get; set; }
    public List<FoodNameViewModel> DislikedFoods { get; set; }

    public PreferencesViewModel()
    {
      FoodGroupPreferences = new List<FoodGroupPreference>();
      RecipeGroupPreferences = new List<RecipeGroupPreference>();
      DislikedFoods = new List<FoodNameViewModel>();
    }
  }
}