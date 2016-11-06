using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public class Diet
  {
    public List<DailyDiet> DailyDiets;

    public Diet()
    {
      DailyDiets = new List<DailyDiet>();
    }
  }
}
