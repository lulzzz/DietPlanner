using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public class Individual
  {
    public List<Individual> Dominated;
    public int DominatedByCount;
    public List<Evaluation> Evaluations;
    public int Rank;
    public double CrowdingDistance;
    public bool IsFeasible;

    public Individual()
    {
      Evaluations = new List<Evaluation>();
      Dominated = new List<Individual>();
    }
  }
}