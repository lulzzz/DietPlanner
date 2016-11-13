using System;
using System.Linq;

namespace DietPlanning.NSGA.MathImplementation
{
  public class MathCrossOver : ICrossOver
  {
    private readonly Random _random;

    public MathCrossOver(Random random)
    {
      _random = random;
    }

    public Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2)
    {
      var mathParent1 = parent1 as MathIndividual;
      var mathParent2 = parent2 as MathIndividual;

      var parent1Gene = ConvertDoubleToByteArray(mathParent1.X1);
      var parent2Gene = ConvertDoubleToByteArray(mathParent2.X1);

      var crossOverPoint = _random.Next(parent1Gene.Length);
      var child1Gene = parent1Gene.Take(crossOverPoint).Concat(parent2Gene.Skip(crossOverPoint)).ToArray();
      var child2Gene = parent2Gene.Take(crossOverPoint).Concat(parent1Gene.Skip(crossOverPoint)).ToArray();

      var child1 = new MathIndividual {X1 = ConvertByteArrayToDouble(child1Gene)};
      var child2 = new MathIndividual {X1 = ConvertByteArrayToDouble(child2Gene)};

      return new Tuple<Individual, Individual>(child1, child2);

    }

    public static byte[] ConvertDoubleToByteArray(double d)
    {
      return BitConverter.GetBytes(d);
    }
    public static double ConvertByteArrayToDouble(byte[] b)
    {
      return BitConverter.ToDouble(b, 0);
    }
  }
}
