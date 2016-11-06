namespace DietPlanning.NSGA
{
  public class Objective
  {
    public ObjectiveType Type;
    public double Scrore;
  }

  public enum ObjectiveType
  {
    Cost,
    Preferences,
    PreparationTime,
    Macro,
    Variety
  }
}