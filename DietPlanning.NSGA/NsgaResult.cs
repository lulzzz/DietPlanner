using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public class NsgaResult
  {
    public List<List<Individual>> Fronts;
    public NsgaLog Log;

    public NsgaResult()
    {
      Fronts = new List<List<Individual>>();
    }
  }
}
