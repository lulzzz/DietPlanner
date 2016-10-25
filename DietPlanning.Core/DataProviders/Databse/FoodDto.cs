using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DietPlanning.Core.DataProviders.Databse
{
  [Table("FOOD_DES", Schema = "public")]
  public class FoodDto
  {
    [Key, ForeignKey("Nutrients"), Column("NDB_No")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

   // [Column("FdGrp_Cd"), ForeignKey("FoodGroup_FK")]
    [Column("FdGrp_Cd")]
    public int FoodGroup { get; set; }

    [Column("Shrt_Desc")]
    public string ShortDescription { get; set; }

    public FoodNutrientsDto Nutrients {get; set;}
  }
}
