using System.Collections.Generic;
using System.Linq;
using DietPlanning.NSGA.GroupDietsImplementation;
using RAdapter;

namespace MultiAttributeDecisionMaking
{
  public class Solver
  {
    public List<GroupDietIndividual> TopsisSort(List<GroupDietIndividual> individuals, WeightsModel topisiModel)
    {
      return RInvoker.Topsis(individuals, topisiModel.CostWeight, topisiModel.PreferncesWeight, topisiModel.PreparationTimeWeight, topisiModel.MacroWeight);
    }

    public List<GroupDietIndividual> AhpSort(List<GroupDietIndividual> individuals, AhpModel ahpModel)
    {
      var invAhp = new AhpModel
      {
        CostMacro = ahpModel.InvertCostMacro ? ahpModel.CostMacro : 1.0/ahpModel.CostMacro,
        PrefTime = ahpModel.InvertPrefTime ? ahpModel.PrefTime : 1.0/ahpModel.PrefTime,
        PrefCost = ahpModel.InvertPrefCost ? ahpModel.PrefCost : 1.0/ahpModel.PrefCost,
        PrefMacro = ahpModel.InvertPrefMacro ? ahpModel.PrefMacro : 1.0/ahpModel.PrefMacro,
        TimeCost = ahpModel.InvertTimeCost ? ahpModel.TimeCost : 1.0/ahpModel.TimeCost,
        TimeMacro = ahpModel.InvertTimeMacro ? ahpModel.TimeMacro : 1.0/ahpModel.TimeMacro
      };

      var matrix = new double[][]
      {
        new[] { 1.0, 1.0 / invAhp.CostMacro, 1.0 / invAhp.TimeMacro, 1.0/ invAhp.PrefMacro},
        new[] { invAhp.CostMacro, 1.0, 1.0 / invAhp.TimeCost, 1.0 / invAhp.PrefCost},
        new[] { invAhp.TimeMacro, invAhp.TimeCost, 1.0, 1.0 / invAhp.PrefTime},
        new[] { invAhp.PrefMacro, 1.0 / invAhp.PrefCost, 1.0 / invAhp.PrefTime, 1.0 }
      };

      var columnSums = new double[4];
      for (int r = 0; r < 4; r++)
      {
        for (var c = 0; c < 4; c++)
        {
          columnSums[c] += matrix[r][c];
        }
      }

      for (int r = 0; r < 4; r++)
      {
        for (var c = 0; c < 4; c++)
        {
          matrix[r][c] = matrix[r][c]/columnSums[c];
        }
      }
      
      //macro, cost, time, preferences
      var priorityVector = new double[4];
      for (int r = 0; r < 4; r++)
      {
        priorityVector[r] = matrix[0].Average();
      }

      var weightModel = new WeightsModel
      {
        MacroWeight = matrix[0].Average(),
        CostWeight = matrix[1].Average(),
        PreparationTimeWeight = matrix[2].Average(),
        PreferncesWeight = matrix[3].Average(),
      };

      var sorted = individuals.OrderBy(i => EuclidianHelper.Euclidian(i, weightModel)).ToList();

      return sorted;
    }

    public List<GroupDietIndividual> EuclideanSort(List<GroupDietIndividual> individuals, WeightsModel weights)
    {
      var sorted = individuals.OrderBy(i => EuclidianHelper.Euclidian(i, weights)).ToList();

      return sorted;
    }
  }
}
