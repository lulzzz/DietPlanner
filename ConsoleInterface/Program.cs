using System;
using System.Linq;

using DietPlanning.Core;
using DietPlanning.Core.DataProviders.Databse;
using DietPlanning.Genetic;
using DietPlanning.Genetic.CrossingOver;
using DietPlanning.Genetic.Selectors;

namespace ConsoleInterface
{
  public class Program
  {
    public static void Main(string[] args)
    {
      const int gaPopulationSize = 100;
      const int gaMaxIterations = 10000;
      const double gaMutationProbability = 0.2;
      const double gaSelectFactor = 0.95;
      const int numberOfMealsPerDay = 5;
      const int numberOfDays = 7;
      const int tournamentSize = 10;

      var random = new Random();
      var dietAnalyzer = new DietAnalyzer();
      var gaConfiguration = new Configuration(gaMutationProbability, gaSelectFactor, numberOfDays, numberOfMealsPerDay, gaPopulationSize, gaMaxIterations);
      var selector = new TournamentSelector(tournamentSize, random);
      var geneticDietSolver = new GeneticDietSolver(new PopulationInitializer(random), selector, new DaysCrossOver(random), new Evaluator(new DietAnalyzer()));

      var foodsProvider = new FoodDatabaseProvider();
      
      var diets = geneticDietSolver.GetDiet(foodsProvider.GetFoods(), GetTargetDiet(), gaConfiguration);

      var summaries = diets.Select(dietAnalyzer.Summarize).ToList();
      Presenter.Output(diets);
      Console.WriteLine(summaries.Count);
     // Console.Read();
    }

    private static DietSummary GetTargetDiet()
    {
      return new DietSummary
      {
        Calories = 3002,
        Fat = 101,
        Carbohydrates = 338,
        Proteins = 188
      };
    }
  }
}
