namespace MultiAttributeDecisionMaking
{
  public class WeightsModel
  {
    public string PersonName { get; set; }
    public int PersonId { get; set; }
    public double CostWeight { get; set; } = 1;
    public double PreferncesWeight { get; set; } = 1;
    public double MacroWeight { get; set; } = 1;
    public double PreparationTimeWeight { get; set; } = 1;
  }
}