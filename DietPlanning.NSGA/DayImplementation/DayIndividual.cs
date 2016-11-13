using DietPlanning.Core.DomainObjects;

namespace DietPlanning.NSGA.DayImplementation
{
  public class DayIndividual : Individual
  {
    public DayIndividual(DailyDiet dailyDiet)
    {
      DailyDiet = dailyDiet;
    }

    public DailyDiet DailyDiet;
  }
}
