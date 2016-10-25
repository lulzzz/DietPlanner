using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DietPlanning.Core.DataProviders.Databse
{
  [Table("NUTRIENTS", Schema = "public")]
  public class FoodNutrientsDto
  {
    [Key, Column("NDB_No")]
    public int FoodId { get; set; }

    [Column("Protein")]
    public double Proteins { get; set; }

    [Column("Lipid_Tot")]
    public double Fat { get; set; }

    [Column("Carbohydrt")]
    public double Carbohydrates { get; set; }
  }
}
