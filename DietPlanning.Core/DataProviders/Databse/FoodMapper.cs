using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders.Databse
{
  internal static class FoodMapper
  {
    private static readonly Dictionary<int, FoodGroup> GroupsMapping = new Dictionary<int, FoodGroup>
    {
      {0100, FoodGroup.DairyAndEggProducts}, //Dairy and Egg Products
      {0200, FoodGroup.SpicesAndHerbs}, //Spices and Herbs
      {0300, FoodGroup.Other},//Baby Foods
      {0400, FoodGroup.FatsOils},//Fats and Oils
      {0500, FoodGroup.Meat},//Poultry Products
      {0600, FoodGroup.SoupsSaucesGravies},//Soups, Sauces, and Gravies
      {0700, FoodGroup.Meat},//Sausages and Luncheon Meats
      {0800, FoodGroup.BreakfastCereals},//Breakfast Cereals
      {0900, FoodGroup.Fruits},//Fruits and Fruit Juices
      {1000, FoodGroup.Meat},//Pork Products
      {1100, FoodGroup.Vegetables},//Vegetables and Vegetable Products
      {1200, FoodGroup.NutAndSeeds},//Nut and Seed Products
      {1300, FoodGroup.Meat},//Beef Products
      {1400, FoodGroup.Beverages},//Beverages
      {1500, FoodGroup.FinfishShellfish},//Finfish and Shellfish Products
      {1600, FoodGroup.Legumes},//Legumes and Legume Products
      {1700, FoodGroup.Meat},//Lamb, Veal, and Game Products
      {1800, FoodGroup.Baked},//Baked Products
      {1900, FoodGroup.Sweets},//Sweets
      {2000, FoodGroup.CerealGrainsAndPasta},//Cereal Grains and Pasta
      {2100, FoodGroup.FastFoods},//Fast Foods
      {2200, FoodGroup.MealsEntreesAndSideDishes},//Meals, Entrees, and Side Dishes
      {2500, FoodGroup.Snacks},//Snacks
      {3500, FoodGroup.Other},//American Indian/Alaska Native Foods
      {3600, FoodGroup.RestaurantFoods}//Restaurant Foods
    };

    public static Food MapToFood(FoodDto foodDto)
    {
      return new Food
      {
        Name = foodDto.ShortDescription,
        Proteins = foodDto.Nutrients.Proteins,
        Fat = foodDto.Nutrients.Fat,
        Carbohydrates = foodDto.Nutrients.Carbohydrates,
        Group = GetGroup(foodDto),
        Id = foodDto.Id
      };
    }

    private static FoodGroup GetGroup(FoodDto foodDto)
    {
      return GroupsMapping[foodDto.FoodGroup];
    }
  }
}
