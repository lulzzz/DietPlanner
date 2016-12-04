using System.Collections.Generic;
using DietPlanning.Core;

namespace DietPlanning.Web.Models
{
  public class PersonDietViewModel
  {
    public DietSummary DietSummary;
    public List<MealViewModel> Meals;

    public PersonDietViewModel()
    {
      Meals = new List<MealViewModel>();
    }
  }

  public class MealViewModel
  {
    public int Calories;
    public bool IsInRange;
    public int DistanceToRange;
    public List<RecipeViewModel> Recipes;

    public MealViewModel()
    {
      Recipes = new List<RecipeViewModel>();
    }
  }

  public class RecipeViewModel
  {
    public string Name;
    public double Amount;
  }
}