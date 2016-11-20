using DietPlanning.Core.NutritionRequirements;
using FluentAssertions;
using NUnit.Framework;

namespace DietPlanning.Core.Tests
{
  [TestFixture]
  public class RangeTests
  {
    [Test]
    public void IsInRange()
    {
      var subject = new Range {Lower = 0.5, Upper = 1.5};

      subject.IsInRange(0.7).Should().BeTrue();
      subject.IsInRange(1.8).Should().BeFalse();
      subject.IsInRange(0.49).Should().BeFalse();
    }
  }
}
