using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.GroupDiets;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;
using RAdapter;

namespace MultiAttributeDecisionMaking
{
  public class Solver
  {
    public List<GroupDietIndividual> TopsisSort(List<GroupDietIndividual> individuals, List<WeightsModel> topisModels)
    {
      var macroMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).Min();
      var prepTimeMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score).Min();
      var costMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score).Min();
      var preferencesMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score).Min();

      var macroMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).Max();
      var prepTimeMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score).Max();
      var costMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score).Max();
      var preferencesMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score).Max();

      var normalized = individuals.Select(i => new GroupDietIndividual(new GroupDiet())
      {
        Evaluations = new List<Evaluation>
        {
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Macro), macroMin, macroMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime), prepTimeMin, prepTimeMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Cost), costMin, costMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences), preferencesMin, preferencesMax)
        },
        Rank = individuals.IndexOf(i)
      });

      var averageTopsis = MakeAverage(topisModels);

      var result = RInvoker.Topsis(normalized.ToList(), averageTopsis.CostWeight, averageTopsis.PreferncesWeight, averageTopsis.PreparationTimeWeight, averageTopsis.MacroWeight);

      return result.Select(i => individuals[i.Rank]).ToList();
    }

    public List<GroupDietIndividual> AhpSort(List<GroupDietIndividual> individuals, List<AhpModel> ahpModels)
    {
      var priorityVectors = ahpModels.Select(GetPriorityVector).ToList();

      var averageWeightModel = new WeightsModel
      {
        MacroWeight = priorityVectors.Select(v => v.MacroWeight).Average(),
        CostWeight = priorityVectors.Select(v => v.CostWeight).Average(),
        PreferncesWeight = priorityVectors.Select(v => v.PreferncesWeight).Average(),
        PreparationTimeWeight = priorityVectors.Select(v => v.PreparationTimeWeight).Average()
      };

      return EuclideanSort(individuals, averageWeightModel);
    }

    public WeightsModel GetPriorityVector(AhpModel ahpModel)
    {
      var invAhp = new AhpModel
      {
        CostMacro = ahpModel.InvertCostMacro ? ahpModel.CostMacro : 1.0 / ahpModel.CostMacro,
        PrefTime = ahpModel.InvertPrefTime ? ahpModel.PrefTime : 1.0 / ahpModel.PrefTime,
        PrefCost = ahpModel.InvertPrefCost ? ahpModel.PrefCost : 1.0 / ahpModel.PrefCost,
        PrefMacro = ahpModel.InvertPrefMacro ? ahpModel.PrefMacro : 1.0 / ahpModel.PrefMacro,
        TimeCost = ahpModel.InvertTimeCost ? ahpModel.TimeCost : 1.0 / ahpModel.TimeCost,
        TimeMacro = ahpModel.InvertTimeMacro ? ahpModel.TimeMacro : 1.0 / ahpModel.TimeMacro
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
          matrix[r][c] = matrix[r][c] / columnSums[c];
        }
      }

      //macro, cost, time, preferences
      var priorityVector = new double[4];
      for (int r = 0; r < 4; r++)
      {
        priorityVector[r] = matrix[0].Average();
      }

      return new WeightsModel
      {
        MacroWeight = matrix[0].Average(),
        CostWeight = matrix[1].Average(),
        PreparationTimeWeight = matrix[2].Average(),
        PreferncesWeight = matrix[3].Average(),
      };
    }

    public List<GroupDietIndividual> EuclideanSort(List<GroupDietIndividual> individuals, List<WeightsModel> weights)
    {
      var average = MakeAverage(weights);

      return EuclideanSort(individuals, average);
    }

    private WeightsModel MakeAverage(List<WeightsModel> weights)
    {
      return new WeightsModel
      {
        MacroWeight = weights.Select(v => v.MacroWeight).Average(),
        CostWeight = weights.Select(v => v.CostWeight).Average(),
        PreferncesWeight = weights.Select(v => v.PreferncesWeight).Average(),
        PreparationTimeWeight = weights.Select(v => v.PreparationTimeWeight).Average()
      };
    }

    private List<GroupDietIndividual> EuclideanSort(List<GroupDietIndividual> individuals, WeightsModel weights)
    {
      var macroMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).Min();
      var prepTimeMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score).Min();
      var costMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score).Min();
      var preferencesMin = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score).Min();

      var macroMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score).Max();
      var prepTimeMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score).Max();
      var costMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score).Max();
      var preferencesMax = individuals.Select(i => i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score).Max();


      var sorted = individuals.OrderBy(i => EuclidianHelper.Euclidian(new GroupDietIndividual(new GroupDiet())
      {
        Evaluations = new List<Evaluation>
        {
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Macro), macroMin, macroMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime), prepTimeMin, prepTimeMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Cost), costMin, costMax),
          Normalize(i.Evaluations.Single(e => e.Type == ObjectiveType.Preferences), preferencesMin, preferencesMax)
        }
      }, weights)).ToList();

      return sorted;
    }

    private Evaluation Normalize(Evaluation evaluation, double min, double max)
    {
      if (min != max)
      {
        return new Evaluation { Type = evaluation.Type, Score = (evaluation.Score - min) / (max - min) };
      }
      else
      {
        return new Evaluation { Type = evaluation.Type, Score = 1 };
      }
    }
  }
}
