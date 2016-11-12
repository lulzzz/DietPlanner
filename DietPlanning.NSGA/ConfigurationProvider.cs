namespace DietPlanning.NSGA
{
  public class ConfigurationProvider
  {
    public Configuration GetConfiguration()
    {
      return new Configuration
      {
        PopulationSize = 200,
        OffspringRatio = 0.5,
        MutationProbability = 0.05,
        MaxIterations = 75
      };
    }
  }
}
