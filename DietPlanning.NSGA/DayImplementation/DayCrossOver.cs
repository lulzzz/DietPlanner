using System;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayCrossOver : ICrossOver
  {
    private readonly Random _random;

    public DayCrossOver(Random random)
    {
      _random = random;
    }

    public Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2)
    {
      var childrenDiets = GetChild(((DayIndividual)parent1).DailyDiet, ((DayIndividual)parent2).DailyDiet);

      return new Tuple<Individual, Individual>(new DayIndividual(childrenDiets.Item1), new DayIndividual(childrenDiets.Item2));
    }

    private Tuple<DailyDiet, DailyDiet> GetChild(DailyDiet parent1, DailyDiet parent2)
    {
      var mealsCrossOverPoint = _random.Next(parent1.Meals.Count);
      var parent1CrossOverMeal = parent1.Meals[mealsCrossOverPoint];
      var parent2CrossOverMeal = parent2.Meals[mealsCrossOverPoint];
      var crossedOverMeals = CrossOverMeal(parent1CrossOverMeal, parent2CrossOverMeal);
      var child1 = new DailyDiet();
      var child2 = new DailyDiet();

      for (var i = 0; i < parent1.Meals.Count; i++)
      {
        var crossOverMeals = CrossOverMeal(parent1.Meals[i], parent2.Meals[i]);
        child1.Meals.Add(crossOverMeals.Item1);
        child2.Meals.Add(crossOverMeals.Item2);
      }

      //child1.Meals.AddRange(parent1.Meals.Take(mealsCrossOverPoint).Select(DietCopier.CopyMeal));
      //child1.Meals.Add(crossedOverMeals.Item1);
      //child1.Meals.AddRange(parent2.Meals.Skip(mealsCrossOverPoint + 1).Select(DietCopier.CopyMeal));

      //child2.Meals.AddRange(parent2.Meals.Take(mealsCrossOverPoint).Select(DietCopier.CopyMeal));
      //child2.Meals.Add(crossedOverMeals.Item2);
      //child2.Meals.AddRange(parent1.Meals.Skip(mealsCrossOverPoint + 1).Select(DietCopier.CopyMeal));

      return new Tuple<DailyDiet, DailyDiet>(child1, child2);
    }

    private Tuple<Meal, Meal> CrossOverMeal(Meal parent1, Meal parent2)
    {
      var crossOverPoint = _random.Next(1 + Math.Min(parent1.Receipes.Count, parent2.Receipes.Count));

      var child1 = new Meal();
      var child2 = new Meal();

      child1.Receipes.AddRange(parent1.Receipes.Take(crossOverPoint));
      child1.Receipes.AddRange(parent2.Receipes.Skip(crossOverPoint));
      child2.Receipes.AddRange(parent2.Receipes.Take(crossOverPoint));
      child2.Receipes.AddRange(parent1.Receipes.Skip(crossOverPoint));

      return new Tuple<Meal, Meal>(child1, child2);
    }
  }
}
