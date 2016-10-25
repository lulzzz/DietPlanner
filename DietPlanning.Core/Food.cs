namespace DietPlanning.Core
{
  public class Food
  {
    public string Name;
    public double Proteins { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }
    public FoodGroup Group { get; set; }
    public int Id { get; set; }

    public Food(){}

    public Food(string name, double proteins, double fat, double carbohydrates)
    {
      Name = name;
      Proteins = proteins;
      Fat = fat;
      Carbohydrates = carbohydrates;
    }
  }
}