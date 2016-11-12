using FluentAssertions;
using NUnit.Framework;

namespace DietPlanning.NSGA.Tests
{
  [TestFixture]
  public class EvaluationUnitTests
  {
    [Test]
    public void ShouldMaximizeEvaluationWithBiggerScoreBeGreater()
    {
      var e1 = new Evaluation {Type = default(ObjectiveType), Direction = Direction.Maximize, Score = 10.0000001};
      var e2 = new Evaluation {Type = default(ObjectiveType), Direction = Direction.Maximize, Score = 10};

      (e1 > e2).Should().BeTrue();
      (e1 < e2).Should().BeFalse();
    }

    [Test]
    public void ShouldMinimizeEvaluationWithSmallerScoreBeGreater()
    {
      var e1 = new Evaluation { Type = default(ObjectiveType), Direction = Direction.Minimize, Score = 10.0000001 };
      var e2 = new Evaluation { Type = default(ObjectiveType), Direction = Direction.Minimize, Score = 10 };

      (e1 < e2).Should().BeTrue();
      (e1 > e2).Should().BeFalse();
    }
  }
}
