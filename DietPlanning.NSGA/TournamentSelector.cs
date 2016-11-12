using System;
using System.Collections.Generic;
using System.Linq;

namespace DietPlanning.NSGA
{
  public class TournamentSelector
  {
    readonly IComparer<Individual> _comparer;
    private readonly int _tournamentSize;
    private readonly Random _random;

    public TournamentSelector(IComparer<Individual> comparer, int tournamentSize, Random random)
    {
      _comparer = comparer;
      _tournamentSize = tournamentSize;
      _random = random;
    }

    public Individual Select(List<Individual> individuals)
    {
      var competetors = new List<Individual>();

      while (competetors.Count < _tournamentSize)
      {
        Individual competetor;
        do
        {
          competetor = individuals[_random.Next(individuals.Count)];
        } while (competetors.Contains(competetor));
        competetors.Add(competetor);
      }

      competetors.Sort(_comparer.Compare);

      return competetors.Last();
    }
  }
}
