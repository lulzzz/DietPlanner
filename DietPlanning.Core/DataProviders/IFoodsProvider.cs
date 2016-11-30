using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders
{
  public interface IFoodsProvider
  {
    List<Food> GetFoods();
  }
}
