namespace DietPlanning.NSGA
{
  public interface IMutator
  {
    void Mutate(Individual individual, double mutationProbability);
  }
}
