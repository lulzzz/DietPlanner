using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.GroupDiets
{
  public class RecipeGroupSplit
  {
    public static readonly List<double> Multipliers = new List<double> { 0.75, 0.85, 1, 1.25, 1.5, 2 };

    public Recipe Recipe { get; set; }
    public List<RecipeAdjustment> Adjustments { get; set; }

    public RecipeGroupSplit()
    {
      Adjustments = new List<RecipeAdjustment>();
    }

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
