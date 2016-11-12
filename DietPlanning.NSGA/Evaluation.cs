using System;

namespace DietPlanning.NSGA
{
  public class Evaluation
  {
    public Direction Direction;
    public ObjectiveType Type;
    public double Score;

    public static bool operator < (Evaluation e1, Evaluation e2)
    {
      if (e1.Direction != e2.Direction || e1.Type != e2.Type)
        throw new ArgumentException("Cannot compare evaluations of different type");

      if (e1.Score == e2.Score)
        return false;

      if (e1.Direction == Direction.Maximize)
        return e1.Score < e2.Score;

      return e1.Score > e2.Score;
    }

    public static bool operator > (Evaluation e1, Evaluation e2)
    {
      if (e1.Direction != e2.Direction || e1.Type != e2.Type)
        throw new ArgumentException("Cannot compare evaluations of different type");

      if (e1.Score == e2.Score)
        return false;

      if (e1.Direction == Direction.Maximize)
        return e1.Score > e2.Score;

      return e1.Score < e2.Score;
    }
  }
}