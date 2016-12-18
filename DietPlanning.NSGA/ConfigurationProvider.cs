 namespace DietPlanning.NSGA
{
  public class ConfigurationProvider
  {
    public Configuration GetConfiguration()
    {
      return new Configuration
      {
        PopulationSize = 100,
        OffspringRatio = 1,
        MutationProbability = 0.002,
        MaxIterations = 150
      };
    }
  }
}
