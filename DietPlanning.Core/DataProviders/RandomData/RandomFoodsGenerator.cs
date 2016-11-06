using System;
using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders.RandomData
{
  public class RandomFoodsGenerator : IFoodsProvider
  {
    private readonly int _numberOfFoodsToGenerate;
    private readonly Random _random;
        
    public RandomFoodsGenerator(int numberOfFoodsToGenerate)
    {
      _random = new Random();
      _numberOfFoodsToGenerate = numberOfFoodsToGenerate;
    }

    public List<Food> GetFoods()
    {
      var meals = new List<Food>();

      for (int i = 0; i < _numberOfFoodsToGenerate; i++)
      {
        var p = GetRandomNumber(0d, 50d, _random);
        var f = GetRandomNumber(0d, 50d, _random);
        var c = GetRandomNumber(0d, 50d, _random);

        meals.Add(new Food(string.Format("FoodNo_{0}", i), p, f, c));
      }

      return meals;
    }

    private double GetRandomNumber(double minimum, double maximum, Random random)
    {
      return random.NextDouble() * (maximum - minimum) + minimum;
    }
  }
}
