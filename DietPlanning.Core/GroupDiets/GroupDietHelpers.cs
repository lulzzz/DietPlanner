using System;
using System.Collections.Generic;
using System.Linq;
using DietPlanning.Core.DomainObjects;
using DietPlanning.Core.NutritionRequirements;

namespace DietPlanning.Core.GroupDiets
{
  public static class GroupDietHelpers
  {
    public static bool ApplyForPerson(this RecipeGroupSplit recipe, int personId)
    {
      return recipe.Adjustments.Any(ad => ad.PersonId == personId);
    }

    public static List<Tuple<double, Recipe>> GetRecipesForPerson(this GroupMeal meal, int personId)
    {
      return
        meal.Recipes.Where(r => r.ApplyForPerson(personId))
          .Select(r => new Tuple<double, Recipe>(r.Adjustments.Single(ad => ad.PersonId == personId).AmountMultiplier, r.Recipe))
          .ToList();
    }

    public static PersonalData ForPersonId(this List<PersonalData> list, int id)
    {
      return list.Single(d => d.Id == id);
    }

    public static List<Recipe> RecipesForPerson(this List<RecipeGroupSplit> list, int id)
    {
      return list.Where(r => r.Adjustments.Any(ad => ad.PersonId == id)).Select(r => r.Recipe).ToList();
    }
  }
}
