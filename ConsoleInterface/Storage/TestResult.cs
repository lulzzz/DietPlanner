using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleInterface.Storage
{
  public class TestResult
  {
    [DataMember]
    public List<ResultPoint> ResultPoints { get; set; }

    [DataMember]
    public int PopulationSize { get; set; }

    [DataMember]
    public double MutationProbability { get; set; }

    [DataMember]
    public double Iterations { get; set; }

    [DataMember]
    public int Time { get; set; }

    public string Name { get; set; }
  }
}
