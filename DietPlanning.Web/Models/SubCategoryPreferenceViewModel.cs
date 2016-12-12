using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Web.Models
{
  public class SubCategoryPreferenceViewModel
  {
    public string DisplayName { get; set; }
    public double Value { get; set; }
    public SubCategory SubCategory { get; set; }
  }
}