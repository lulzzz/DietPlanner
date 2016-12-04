using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.Web.Models
{
  public class GroupDietsResultViewModel
  {
    public List<GroupDietViewModel> GroupDiets;
    public List<PersonalData> PersonalDatas;
  }
}