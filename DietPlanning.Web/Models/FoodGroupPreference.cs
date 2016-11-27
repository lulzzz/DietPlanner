using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Web.Models
{
  public class FoodGroupPreference
  {
    public FoodGroup Group { get; set; }
    public int Preference { get; set; }
  }
}