namespace DietPlanning.NSGA
{
  public class ConfigurationProvider
  {
    public Configuration GetConfiguration()
    {
      return new Configuration
      {
        PopulationSize = 200,
        NumberOfDays = 7,
        NumberOfMealsPerDay = 5,
        OffspringRatio = 0.3,
        MutationProbability = 0.01,
        MaxIterations = 100
      };
    }
  }
}
