using System.Data.Entity;

namespace DietPlanning.Core.DataProviders.Databse
{
  public class FoodDatabseContext : DbContext
  {
    public DbSet<FoodDto> FoodSet { get; set; }

    public FoodDatabseContext()
        : base("NutritionDb")
    {
      //disable initializer
      Database.SetInitializer<FoodDatabseContext>(null);
    }
  }
}
