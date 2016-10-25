using System;
using System.Linq;

using DietPlanning.Core;

namespace DietPlanning.Genetic
{
  public class Evaluator
  {
    private readonly DietAnalyzer _dietAnalyzer;

    public Evaluator(DietAnalyzer dietAnalyzer)
    {
      _dietAnalyzer = dietAnalyzer;
    }

    public double Evaluate(Diet diet, DietSummary targetDailyDiet)
    {
      return diet.DailyDiets.Select(dailyDiet => EvaluateDaily(dailyDiet, targetDailyDiet)).Sum() / diet.DailyDiets.Count;
    }

    private double EvaluateDaily(DailyDiet dailyDiet, DietSummary targetDailyDiet)
    {
      var dailySummary = _dietAnalyzer.SummarizeDaily(dailyDiet);

      var distance =
        Math.Abs(targetDailyDiet.Proteins - dailySummary.Proteins) +
        Math.Abs(targetDailyDiet.Fat - dailySummary.Fat) +
        Math.Abs(targetDailyDiet.Calories - dailySummary.Calories) +
        Math.Abs(targetDailyDiet.Carbohydrates - dailySummary.Carbohydrates);

      return distance;
      //distance > 0 
      //? 1 / (distance) 
      //: 1;
    }
  }
}
