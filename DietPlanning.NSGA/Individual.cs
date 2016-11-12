using System;
using System.Collections.Generic;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  public class Individual
  {
    public List<Individual> Dominated;
    public int DominatedByCount;
    //public Diet Diet;
    public List<Evaluation> Evaluations;
    public int Rank;
    public double CrowdingDistance;

    public Individual()
    {
      Evaluations = new List<Evaluation>();
      Dominated = new List<Individual>();
    }
  }
}