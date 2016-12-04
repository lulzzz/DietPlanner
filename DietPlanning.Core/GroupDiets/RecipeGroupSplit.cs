using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.GroupDiets
{
  public class RecipeGroupSplit
  {
    public Recipe Recipe { get; set; }
    public List<RecipeAdjustment> Adjustments { get; set; }

    public RecipeGroupSplit(int groupSize)
    {
      Adjustments = new List<RecipeAdjustment>();

      for (int i = 0; i < groupSize; i++)
      {
        Adjustments.Add(new RecipeAdjustment {AmountMultiplier = 1.0, PersonId = i});
      }
    }
  }
}
