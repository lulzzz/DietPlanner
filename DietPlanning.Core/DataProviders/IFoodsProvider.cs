using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders
{
  interface IFoodsProvider
  {
    List<Food> GetFoods();
  }
}
