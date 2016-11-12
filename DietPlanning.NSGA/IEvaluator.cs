using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public interface IEvaluator
  {
    void Evaluate(List<Individual> individuals);
    void Evaluate(Individual individual);
  }
}
