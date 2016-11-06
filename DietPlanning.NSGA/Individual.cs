using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class Individual
  {
    public List<Individual> Dominated; //Sp
    public int DominatedByCount; // np
    public Diet Diet;
    public Evaluation Evaluation;

    public Individual(Diet diet)
    {
      Evaluation = new Evaluation();
      Dominated = new List<Individual>();
      Diet = diet;
    }
  }
}