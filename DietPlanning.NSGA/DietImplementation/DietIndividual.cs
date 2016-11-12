using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DietImplementation
{
  public class DietIndividual : Individual
  {
    public Diet Diet;

    public DietIndividual(Diet diet)
    {
      Diet = diet;
    }
  }
}
