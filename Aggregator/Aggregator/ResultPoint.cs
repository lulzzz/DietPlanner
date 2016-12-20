using System.Runtime.Serialization;

namespace Aggregator
{
  [DataContract]
  public class ResultPoint
  {
    [DataMember(Name = "M")]
    public double Macro { get; set; }

    [DataMember(Name = "C")]
    public double Cost { get; set; }

    [DataMember(Name = "PR")]
    public double Preferences { get; set; }

    [DataMember(Name = "PT")]
    public double PreparationTime { get; set; }
  }
}
