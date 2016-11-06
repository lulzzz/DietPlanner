using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA
{
  class Program
  {
    static void Main(string[] args)
    {
      var solver = new NsgaSolver(new Sorter());

      //TODO its empty
      var diets = new List<Diet>();

      var fronts = solver.Solve(diets);

      Console.WriteLine(fronts.Select(f => f.Count).Sum());

      Console.ReadKey();
    }
  }
}