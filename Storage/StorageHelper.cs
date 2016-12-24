using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DietPlanning.Core.GroupDiets;
using DietPlanning.NSGA;
using DietPlanning.NSGA.GroupDietsImplementation;
using Newtonsoft.Json;
using Storage;

namespace ConsoleInterface.Storage
{
  public static class StorageHelper
  {
    public static ResultPoint CreateResultPoint(GroupDietIndividual individual)
    {
      return new ResultPoint
      {
        Cost = individual.Evaluations.Single(e => e.Type == ObjectiveType.Cost).Score,
        Preferences = individual.Evaluations.Single(e => e.Type == ObjectiveType.Preferences).Score,
        PreparationTime = individual.Evaluations.Single(e => e.Type == ObjectiveType.PreparationTime).Score,
        Macro = individual.Evaluations.Single(e => e.Type == ObjectiveType.Macro).Score
        //Diet = CreateDietData(individual)
      };
    }

    private static DietData CreateDietData(GroupDietIndividual individual)
    {
      return new DietData {Meals = individual.GroupDiet.Meals.Select(CreateMealData).ToList()};
    }

    private static MealData CreateMealData(GroupMeal meal)
    {
      return new MealData {Recipes = CreateRecipesData(meal.Recipes)};
    }

    private static List<RecipeData> CreateRecipesData(List<RecipeGroupSplit> mealRecipes)
    {
      return mealRecipes.Select(CreateRecipeData).ToList();
    }

    private static RecipeData CreateRecipeData(RecipeGroupSplit recipeGroupSplit)
    {
      return new RecipeData
      {
        Id = recipeGroupSplit.Recipe.Id,
        Splits =
          recipeGroupSplit.Adjustments.Select(
            ad => new SplitData {Multiplier = ad.AmountMultiplier, PersonId = ad.PersonId}).ToList()
      };
    }

    public static FrontResult CreatTestResult(NsgaResult nsgaResult, Configuration configuration)
    {
      var testResult = new FrontResult
      {
        Iterations = configuration.MaxIterations,
        PopulationSize = configuration.PopulationSize,
        MutationProbability = configuration.MutationProbability,
        Time = nsgaResult.Log.SolvingTime,
        ResultPoints = nsgaResult.Fronts.SelectMany(f => f).Select(g => CreateResultPoint((GroupDietIndividual) g)).ToList(),
        SeriesName = CreateName(configuration)
      };

      return testResult;
    }

    public static string CreateName(Configuration configuration)
    {
      var time = DateTime.Now;

      return time.ToString(CultureInfo.InvariantCulture).Replace("/", "").Replace(" ", "_").Replace(":", "") + "_" + time.Millisecond
       + "_i" + configuration.MaxIterations + "_m" + configuration.MutationProbability.ToString(CultureInfo.InvariantCulture).Replace(".", "") + "_p" + configuration.PopulationSize;
    }

    public static void SaveAsJson(string outputPath, string seriesName, FrontResult testResult)
    {
      var json = JsonConvert.SerializeObject(testResult);

      var saveDirectory = outputPath + "\\" +seriesName;

      if (!Directory.Exists(saveDirectory))
      {
        Directory.CreateDirectory(saveDirectory);
      }

      File.WriteAllText(saveDirectory + "\\" + testResult.SeriesName + ".json", json);
    }

    public static FrontResult ReadFromJson(string outputPath, string seriesName, string filename)
    {
      var saveDirectory = outputPath + "\\" + seriesName;

      if (!Directory.Exists(saveDirectory))
      {
        return null;
      }

      return JsonConvert.DeserializeObject<FrontResult>(File.ReadAllText(saveDirectory + "\\" + filename));
    }
  }
}
