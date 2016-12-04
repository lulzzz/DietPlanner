﻿using System;
using System.Linq;
using DietPlanning.Core.GroupDiets;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public class GroupDietCrossOver : ICrossOver
  {
    private readonly Random _random;

    public GroupDietCrossOver(Random random)
    {
      _random = random;
    }

    public Tuple<Individual, Individual> CreateChildren(Individual parent1, Individual parent2)
    {
      var childrenDiets = GetChild(((GroupDietIndividual)parent1).GroupDiet, ((GroupDietIndividual)parent2).GroupDiet);

      return new Tuple<Individual, Individual>(new GroupDietIndividual(childrenDiets.Item1), new GroupDietIndividual(childrenDiets.Item2));
    }

    private Tuple<GroupDiet, GroupDiet> GetChild(GroupDiet parent1, GroupDiet parent2)
    {
      var child1 = new GroupDiet();
      var child2 = new GroupDiet();

      for (var i = 0; i < parent1.Meals.Count; i++)
      {
        var crossOverMeals = CrossOverMeal(parent1.Meals[i], parent2.Meals[i]);
        child1.Meals.Add(crossOverMeals.Item1);
        child2.Meals.Add(crossOverMeals.Item2);
      }

      return new Tuple<GroupDiet, GroupDiet>(child1, child2);
    }

    private Tuple<GroupMeal, GroupMeal> CrossOverMeal(GroupMeal parent1, GroupMeal parent2)
    {
      var crossOverPoint = _random.Next(1 + Math.Min(parent1.Recipes.Count, parent2.Recipes.Count));

      var child1 = new GroupMeal();
      var child2 = new GroupMeal();

      child1.Recipes.AddRange(parent1.Recipes.Take(crossOverPoint));
      child1.Recipes.AddRange(parent2.Recipes.Skip(crossOverPoint));
      child2.Recipes.AddRange(parent2.Recipes.Take(crossOverPoint));
      child2.Recipes.AddRange(parent1.Recipes.Skip(crossOverPoint));

      RemoveRepeatingRecipes(child1);
      RemoveRepeatingRecipes(child2);

      return new Tuple<GroupMeal, GroupMeal>(child1, child2);
    }

    private void RemoveRepeatingRecipes(GroupMeal meal)
    {
      for (var i = 0; i < meal.Recipes.Count; i++)
      {
        for (var j = meal.Recipes.Count - 1; j > i; j--)
        {
          if (meal.Recipes[i] == meal.Recipes[j])
          {
            meal.Recipes.RemoveAt(j);
          }
        }
      }
    }
  }
}
