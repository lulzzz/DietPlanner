using DietPlanning.Core.GroupDiets;

namespace DietPlanning.NSGA.GroupDietsImplementation
{
  public class GroupDietIndividual : Individual
  {
    public GroupDiet GroupDiet;

    public GroupDietIndividual(GroupDiet groupDiet)
    {
      GroupDiet = groupDiet;
    }
  }
}
