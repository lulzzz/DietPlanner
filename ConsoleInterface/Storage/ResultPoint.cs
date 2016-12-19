using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleInterface.Storage
{
  [DataContract]
  public class ResultPoint
  {
    [DataMember(Name = "M")]
    public double Macro { get; set; }

    [DataMember(Name = "PT")]
    public double PreparationTime { get; set; }

    [DataMember(Name = "C")]
    public double Cost { get; set; }

    [DataMember(Name = "PR")]
    public double Preferences { get; set; }

    //[DataMember(Name = "DT")]
    //public DietData Diet { get; set; }
  }

  [DataContract]
  public class DietData
  {
    [DataMember(Name = "Mls")]
    public List<MealData> Meals { get; set; }
  }

  [DataContract]
  public class MealData
  {
    [DataMember(Name = "Rps")]
    public List<RecipeData> Recipes { get; set; }
  }

  [DataContract]
  public class RecipeData
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember(Name = "Sp")]
    public List<SplitData> Splits { get; set; }
  }

  [DataContract]
  public class SplitData
  {
    [DataMember(Name = "Pid")]
    public int PersonId { get; set; }

    [DataMember(Name = "Mlt")]
    public double Multiplier { get; set; }
  }
}
