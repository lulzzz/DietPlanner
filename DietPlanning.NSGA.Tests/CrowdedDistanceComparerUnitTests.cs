using System;
using DietPlanning.Core.DomainObjects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace DietPlanning.NSGA.Tests
{
  [TestClass]
  public class CrowdedDistanceComparerUnitTests
  {
    private CrowdedDistanceComparer _subject;
    private Individual _i1;
    private Individual _i2;

    [SetUp]
    public void SetUp()
    {
      _subject = new CrowdedDistanceComparer();
      _i1 = new Individual();
      _i2 = new Individual();
    }

    [Test]
    public void IndividualWithHigherRankShouldBeGreater()
    {
      _i1.CrowdingDistance = 10;
      _i2.CrowdingDistance = 1;

      _i1.Rank = 1;
      _i2.Rank = 10;

      _subject.Compare(_i1, _i2).Should().Be(1);
      _subject.Compare(_i2, _i1).Should().Be(-1);
    }

    [Test]
    public void WhenRanksAreTheSame_IndividualWithHigherCrowdingDistanceShouldBeGreater()
    {
      _i1.CrowdingDistance = 10;
      _i2.CrowdingDistance = 1;

      _i1.Rank = 1;
      _i2.Rank = 1;

      _subject.Compare(_i1, _i2).Should().Be(1);
      _subject.Compare(_i2, _i1).Should().Be(-1);
    }

    [Test]
    public void WhenRanksAndCrowdingDistancesAreTheSame_ShouldEquality()
    {
      _i1.CrowdingDistance = 1;
      _i2.CrowdingDistance = 1;

      _i1.Rank = 1;
      _i2.Rank = 1;

      _subject.Compare(_i1, _i2).Should().Be(0);
    }
  }
}
