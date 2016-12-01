namespace DietPlanning.Core.DomainObjects
{
  public class Food
  {
    public string Name;
    public string Category { get; set; }
    public int Id { get; set; }
    public NutritionValues NutritionValues;

    public Food(){}

    public Food(string name, double proteins, double fat, double carbohydrates)
    {
      NutritionValues = new NutritionValues();
      Name = name;
      NutritionValues.Proteins = proteins;
      NutritionValues.Fat = fat;
      NutritionValues.Carbohydrates = carbohydrates;
    }
  }
}