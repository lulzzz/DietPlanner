using System.Collections.Generic;
using DietPlanning.Core;

namespace DietPlanning.NSGA
{
  public class Individual
  {
    public List<Individual> Dominated; //Sp
    public int DominatedByCount; // np
    public Diet Diet;
    public List<Objective> Objectives;

    public Individual(Diet diet)
    {
      Objectives = new List<Objective>();
      Dominated = new List<Individual>();
      Diet = diet;
    }
  }
}