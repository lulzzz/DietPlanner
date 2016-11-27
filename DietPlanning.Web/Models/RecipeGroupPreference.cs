using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Web.Models
{
  public class RecipeGroupPreference
  {
    public RecipeGroup Group { get; set; }
    public int Preference { get; set; }
  }
}