namespace DietPlanning.Genetic
{
  public class Configuration
  {
    public double MutationProbability;
    public int NumberOfDays;
    public int NumberOfMealsPerDay;
    public int PopulationSize;
    public double SelectionFactor;
    public int MaxIterations;


    public Configuration(double mutationProbability, double selectionFactor, int numberOfDays, int numberOfMealsPerDay, int populationSize, int maxIterations)
    {
      MutationProbability = mutationProbability;
      NumberOfDays = numberOfDays;
      NumberOfMealsPerDay = numberOfMealsPerDay;
      PopulationSize = populationSize;
      MaxIterations = maxIterations;
      SelectionFactor = selectionFactor;
    }
  }
}
