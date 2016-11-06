using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders
{
  public interface IRecipeProvider
  {
    List<Recipe> GetRecipes();
  }
}