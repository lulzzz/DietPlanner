using System.Collections.Generic;

namespace DietPlanning.NSGA.MathImplementation
{
  public class MathEvaluator : IEvaluator
  {
    public void Evaluate(List<Individual> individuals)
    {
      individuals.ForEach(Evaluate);
    }

    public void Evaluate(Individual individual)
    {
      var mathIndividual = individual as MathIndividual;

      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Cost, Score = f1(mathIndividual.X1)});
      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Macro, Score = f2(mathIndividual.X1, mathIndividual.X2)});
    }

    private double f1(double x)
    {
      return x;
    }

    private double f2(double x1, double x2)
    {
      return (1 + x1)/x2;
    }
  }
}
