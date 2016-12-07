using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public class NsgaLog
  {
    public List<int> FrontsNumberLog;
    public List<int> FirstFrontSizeLog;
    public List<ObjectiveLog> ObjectiveLogs;
    public List<double> CrowdingDistanceVar;
    public List<double> CrowdingDistanceAvg;
    public List<double> FeasibleSolutions;

    public NsgaLog()
    {
      ObjectiveLogs = new List<ObjectiveLog>();
      FrontsNumberLog = new List<int>();
      FirstFrontSizeLog = new List<int>();
      CrowdingDistanceVar = new List<double>();
      CrowdingDistanceAvg = new List<double>();
      FeasibleSolutions = new List<double>();
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
