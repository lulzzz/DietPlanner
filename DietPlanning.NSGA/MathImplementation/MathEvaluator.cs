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
      mathIndividual.Evaluations.Clear();
      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Cost, Direction = Direction.Minimize, Score = f1(mathIndividual.X1)});
      mathIndividual.Evaluations.Add(new Evaluation {Type = ObjectiveType.Macro, Direction = Direction.Minimize, Score = f2(mathIndividual.X1)});
    }

    private bool CheckIfFeasible(MathIndividual individual)
    {
      return!(individual.X1 < 0.5 || individual.X1 > 1.5);

    }

    private double f1(double x)
    {
      return x*x;
    }

    private double f2(double x)
    {
      return (x-2)*(x-2);
    }
  }
}
