using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;
using FluentAssertions;
using NUnit.Framework;

namespace DietPlanning.NSGA.Tests
{
  public class SorterUnitTests
  {
    private Sorter _subject;

    [SetUp]
    public void SetUp()
    {
      _subject = new Sorter();
    }

    [Test]
    public void ShouldReturnFronts()
    {
      var individuals = new List<Individual>();

      _subject.Sort(individuals).Should().BeOfType <List<List<Individual>>>();
    }

    [Test]
    public void ShouldPutDominatedSolutionsInDifferentFronts()
    {
     
      var individual1 = new Individual(new Diet());
      var individual2 = new Individual(new Diet());
      var individuals = new List<Individual> {individual1, individual2};
      individual1.Evaluations.Add(new Evaluation {Score = 1.0, Type = ObjectiveType.Cost});
      individual1.Evaluations.Add(new Evaluation {Score = 1.0, Type = ObjectiveType.Macro});
      individual2.Evaluations.Add(new Evaluation {Score = 2.0, Type = ObjectiveType.Cost});
      individual2.Evaluations.Add(new Evaluation {Score = 1.0, Type = ObjectiveType.Macro});

      _subject.Sort(individuals).Count.Should().Be(2);
    }

    [Test]
    public void ShouldPutNonDominatedSolutionsInSameFront()
    {

      var individual1 = new Individual(new Diet());
      var individual2 = new Individual(new Diet());
      var individuals = new List<Individual> { individual1, individual2 };
      individual1.Evaluations.Add(new Evaluation { Score = 1.0, Type = ObjectiveType.Cost });
      individual2.Evaluations.Add(new Evaluation { Score = 1.0, Type = ObjectiveType.Cost });

      _subject.Sort(individuals).Count.Should().Be(1);
    }
  }
}
