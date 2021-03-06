﻿ namespace DietPlanning.NSGA
{
  public class ConfigurationProvider
  {
    public Configuration GetConfiguration()
    {
      return new Configuration
      {
        PopulationSize = 150,
        OffspringRatio = 1,
        MutationProbability = 0.02,
        MaxIterations = 190
      };
    }
  }
}
