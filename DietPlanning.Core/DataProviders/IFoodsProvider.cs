using System.Collections.Generic;

namespace DietPlanning.Core.DataProviders
{
  interface IFoodsProvider
  {
    List<Food> GetFoods();
  }
}
