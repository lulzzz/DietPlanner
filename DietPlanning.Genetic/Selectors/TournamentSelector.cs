using System;
using System.Collections.Generic;
using System.Linq;

using DietPlanning.Core;

namespace DietPlanning.Genetic.Selectors
{
  public class TournamentSelector : ISelector
  {
    private readonly int _tournamentSize;
    private readonly Random _random;

    public TournamentSelector(int tournamentSize, Random random)
    {
      _tournamentSize = tournamentSize;
      _random = random;
    }

    public List<Diet> Select(List<KeyValuePair<Diet, double>> evaluatedPopulation, int numberOfIndividualsToSelect)
    {
      var selecteDiets = new List<Diet>();

      while (selecteDiets.Count < numberOfIndividualsToSelect)
      {
        var individualsForTournament = SelectRandomIndividualsForTournament(evaluatedPopulation);
        var tournamentWinner = MakeTournament(individualsForTournament);
        evaluatedPopulation.Remove(tournamentWinner);
        selecteDiets.Add(tournamentWinner.Key);
      }

      return selecteDiets;
    }

    private KeyValuePair<Diet, double> MakeTournament(List<KeyValuePair<Diet, double>> individualsForTournament)
    {
      return individualsForTournament.OrderBy(evaluation => evaluation.Value).ToList().First();
    }

    private List<KeyValuePair<Diet, double>> SelectRandomIndividualsForTournament(List<KeyValuePair<Diet, double>> evaluatedPopulation)
    {
      var individualsForTournament = new List<KeyValuePair<Diet, double>>();

      for (int i = 0; i < _tournamentSize; i++)
      {
        individualsForTournament.Add(evaluatedPopulation[_random.Next(evaluatedPopulation.Count)]);
      }

      return individualsForTournament;
    }
  }
}
