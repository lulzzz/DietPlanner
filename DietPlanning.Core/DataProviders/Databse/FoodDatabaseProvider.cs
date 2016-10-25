using System.Collections.Generic;
using System.Linq;

namespace DietPlanning.Core.DataProviders.Databse
{
  public class FoodDatabaseProvider : IFoodsProvider
  {
    public List<Food> GetFoods()
    {
      var dbContext = new FoodDatabseContext();

      return dbContext.FoodSet.Include("Nutrients").Select(FoodMapper.MapToFood).ToList();
    }
  }
}
