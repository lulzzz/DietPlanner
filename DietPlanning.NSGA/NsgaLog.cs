using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public class NsgaLog
  {
    public List<int> FrontsNumberLog;
    public List<int> FirstFrontSizeLog;
    public List<ObjectiveLog> ObjectiveLogs;

    public NsgaLog()
    {
      ObjectiveLogs = new List<ObjectiveLog>();
      FrontsNumberLog = new List<int>();
      FirstFrontSizeLog = new List<int>();
    }
  }

  public class ObjectiveLog
  {
    public ObjectiveType ObjectiveType;
    public double Avg;
    public double Min;
    public double Max;

    public int Iteration;
  }
}
