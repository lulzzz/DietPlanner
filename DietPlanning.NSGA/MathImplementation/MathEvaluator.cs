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
      individual.IsFeasible = CheckIfFeasible(mathIndividual);

      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Cost, Direction = Direction.Minimize, Score = f1(mathIndividual.X1)});
      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Macro, Direction = Direction.Minimize, Score = f2(mathIndividual.X1, mathIndividual.X2)});
    }

    private bool CheckIfFeasible(MathIndividual individual)
    {
      return 
        individual.X1 < 0.1 || 
        individual.X1 > 1 || 
        individual.X2 < 0 || 
        individual.X2 > 5;
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
