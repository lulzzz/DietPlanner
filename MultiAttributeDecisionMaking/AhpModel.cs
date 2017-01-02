namespace MultiAttributeDecisionMaking
{
  public class AhpModel
  {
    public double CostMacro { get; set; } = 1;
    public double TimeMacro { get; set; } = 1;
    public double TimeCost { get; set; } = 1;
    public double PrefMacro { get; set; } = 1;
    public double PrefCost { get; set; } = 1;
    public double PrefTime { get; set; } = 1;
    public bool InvertCostMacro { get; set; } = false;
    public bool InvertTimeMacro { get; set; } = false;
    public bool InvertTimeCost { get; set; } = false;
    public bool InvertPrefMacro { get; set; } = false;
    public bool InvertPrefCost { get; set; } = false;
    public bool InvertPrefTime { get; set; } = false;
  }
}