using System.Collections.Generic;

namespace DietPlanning.Core
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
