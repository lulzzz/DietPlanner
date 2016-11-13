namespace DietPlanning.NSGA
{
  public class ConfigurationProvider
  {
    public Configuration GetConfiguration()
    {
      return new Configuration
      {
        PopulationSize = 200,
        OffspringRatio = 1,
        MutationProbability = 0.5,
        MaxIterations = 300
      };
    }
  }
}
