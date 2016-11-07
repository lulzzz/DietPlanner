using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Genetic.AHP
{
  public class AhpSolver
  {
    private static Random _random;

    public AhpSolver()
    {
      _random = new Random();
    }

    public List<KeyValuePair<Diet, double>> Evaluate(List<Diet> alternatives, DietSummary targetDailyDiet)
    {
      var criteries = new List<Criterion> { new Criterion(alternatives, targetDailyDiet), new Criterion(alternatives, targetDailyDiet), new Criterion(alternatives, targetDailyDiet), new Criterion(alternatives, targetDailyDiet) };
      var criteryMatrix = CreateSquareMatrix(criteries.Count);
      var comparsionMatrices = CreateComparsionMatrices(criteries, alternatives, targetDailyDiet);

      var criteriesPriorityVector = GetPriorityVector(criteryMatrix);
      var alternativesPriorityVectors = comparsionMatrices.Select(matrix => GetPriorityVector(matrix.Value)).ToArray();

      return GetRanking(alternatives, criteriesPriorityVector, alternativesPriorityVectors);
    }

    private List<KeyValuePair<Diet, double>> GetRanking(List<Diet> alternatives, double[] criteriesPriorityVector, double[][] alternativesPriorityVectors)
    {
      var ranking = new List<KeyValuePair<Diet, double>>();

      for (int alternativeIndex = 0; alternativeIndex < alternatives.Count; alternativeIndex++)
      {
        ranking.Add(new KeyValuePair<Diet, double>(alternatives[alternativeIndex], Product(criteriesPriorityVector, alternativesPriorityVectors, alternativeIndex)));
      }

      return ranking;
    }

    private double Product(double[] criteriesPriorityVector, double[][] alternativesPriorityVector, int alternativeIndex)
    {
      var product = 0.0;

      for (int i = 0; i < criteriesPriorityVector.Length; i++)
      {
        product += criteriesPriorityVector[i]*alternativesPriorityVector[i][alternativeIndex];
      }

      return product;
    }

    private static Dictionary<Criterion, double[,]> CreateComparsionMatrices(List<Criterion> criteries, List<Diet> alternatives, DietSummary targetDailyDiet)
    {
      var comparsionMatrices = new Dictionary<Criterion, double[,]>();
      criteries.ForEach(criterion => comparsionMatrices.Add(criterion, CreateSquareMatrix(alternatives.Count)));
      foreach (var alternative in alternatives)
      {
        foreach (var comparingAlternative in alternatives)
        {
          criteries.ForEach(c => comparsionMatrices[c][alternatives.IndexOf(alternative), alternatives.IndexOf(comparingAlternative)] = c.Compare(alternative, comparingAlternative));
        }
      }
      return comparsionMatrices;
    }

    private static double[] GetPriorityVector(double[,] matrix)
    {
      var numberOfRows = matrix.GetLength(1);
      var numberOfColumns = matrix.GetLength(0);
      var columnSums = new double[numberOfColumns];
      var priorityVector = new double[numberOfRows];

      for (int column = 0; column < numberOfColumns; column++)
      {
        for (int row = 0; row < numberOfRows; row++)
        {
          columnSums[column] += matrix[column, row];
        }
      }

      for (int row = 0; row < numberOfRows; row++)
      {
        for (int column = 0; column < numberOfColumns; column++)
        {
          priorityVector[row] += matrix[column, row] / columnSums[column];
        }
        priorityVector[row] = priorityVector[row] / numberOfColumns;
      }

      return priorityVector;
    }

    private static double[,] CreateSquareMatrix(int size)
    {
      var matrix = new double[size, size];
      for (var i = 0; i < size; i++)
      {
        for (var j = 0; j < size; j++)
        {
          matrix[i, j] = _random.NextDouble(); //RANDOM
        }
      }
      return matrix;
    }
  }

  public class Criterion
  {
    private readonly List<KeyValuePair<Diet, double>> _evaluations;

    public Criterion(List<Diet> alternatives, DietSummary targetDailyDiet)
    {
      var evaluator = new Evaluator(new DietAnalyzer());
      _evaluations =
        alternatives.Select(
          alternative => new KeyValuePair<Diet, double>(alternative, evaluator.Evaluate(alternative, targetDailyDiet))).ToList();
    }

    public double Compare(Diet alternative1, Diet alternative2)
    {
      return _evaluations.Single(ev => ev.Key == alternative1).Value -
             _evaluations.Single(ev => ev.Key == alternative2).Value;
    }
  }
}
