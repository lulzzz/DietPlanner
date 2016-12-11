using System.Collections.Generic;
using DietPlanning.Core.DomainObjects;

namespace DietPlanning.Core.DataProviders.Csv
{
  public class FoodGroupsMapper
  {
    public static Dictionary<string, MainCategory> MainCategoryCsvMapper = new Dictionary<string, MainCategory>
    {
      {"Desery", MainCategory.Desery},
      {"Mieso i przetwory miesne", MainCategory.MiesoIPrzetworyMiesne},
      {"Mleko i przetwory mleczne", MainCategory.MlekoIPrzetworyMleczne},
      {"Napoje", MainCategory.Napoje},
      {"Napoje alkoholowe", MainCategory.Napoje},
      {"owoce i przetwory owocowe", MainCategory.OwoceIPrzetworyOwocowe},
      {"owoce swieze", MainCategory.OwoceSwieze},
      {"Potrawy miesne", MainCategory.PotrawyMiesne},
      {"Potrawy rybne", MainCategory.PotrawyRybne},
      {"Potrawy warzywno-miesne", MainCategory.PotrawyWarzywnoMiesne},
      {"Potrawy z jaj", MainCategory.PotrawyZJaj},
      {"Potrawy z kasz, maki, ziemniakow", MainCategory.PotrawyZKaszMakiZiemniakow},
      {"Potrawy z warzyw", MainCategory.PotrawyZWarzyw},
      {"Produkty zbozowe", MainCategory.ProduktyZbozowe},
      {"Przekaski", MainCategory.Przekaski},
      {"Przystawki", MainCategory.Przystawki},
      {"Ryby i przetwory rybne", MainCategory.RybyIPrzetworyRybne},
      {"warzywa i przetwory warzywne", MainCategory.WarzywaIPrzetworyWarzywne},
      {"Zupy", MainCategory.Zupy}
    };

    public static Dictionary<string, SubCategory> SubCategoryCsvMapper = new Dictionary<string, SubCategory>
    {
      {"owoce swieze", SubCategory.OwoceSwieze},
      {"Wypieki", SubCategory.Wypieki},
      {"Wedliny wieprzowe, wolowe i mieszane", SubCategory.WedlinyWieprzoweWoloweIMieszane},
      {"Zupy", SubCategory.Zupy},
      {"mieszanki warzywne mro?one", SubCategory.MieszankiWarzywneMrozone},
      {"slodycze", SubCategory.Slodycze},
      {"Potrawy z wolowiny", SubCategory.PotrawyZWolowiny},
      {"Potrawy warzywno-miesne", SubCategory.PotrawyWarzywnoMiesne},
      {"Potrawy z wieprzowiny", SubCategory.PotrawyZWieprzowiny},
      {"Potrawy z kasz, maki, ziemniakow", SubCategory.PotrawyZKaszMakiZiemniakow},
      {"Warzywa gotowane", SubCategory.WarzywaGotowane},
      {"Desery mleczne", SubCategory.DeseryMleczne},
      {"mieszanki warzywne mrozone", SubCategory.MieszankiWarzywneMrozone},
      {"Pieczywo pszenne polcukiernicze", SubCategory.PieczywoPszennePolcukiernicze},
      {"Pieczywo pszenne", SubCategory.PieczywoPszenne},
      {"Pieczywo mieszane", SubCategory.PieczywoMieszane},
      {"Pieczywo pszenne wyborowe", SubCategory.PieczywoPszenneWyborowe},
      {"Chipsy", SubCategory.Chipsy},
      {"Pieczywo zytnie", SubCategory.PieczywoZytnie},
      {"Ciastka", SubCategory.Ciastka},
      {"Potrawy z cieleciny", SubCategory.PotrawyZCieleciny},
      {"Slodycze", SubCategory.Slodycze},
      {"Potrawy rybne", SubCategory.PotrawyRybne},
      {"dzemy, konfitury, marmolady, powidla", SubCategory.DzemyKonfituryMarmoladyPowidla},
      {"owoce suszone", SubCategory.OwoceSuszone},
      {"Desery", SubCategory.Desery},
      {"Potrawy z jaj", SubCategory.PotrawyZJaj},
      {"Warzywa", SubCategory.Warzywa},
      {"Napoje mleczne", SubCategory.NapojeMleczne},
      {"Warzywa ", SubCategory.Warzywa},
      {"Zupy mleczne", SubCategory.ZupyMleczne},
      {"Potrawy z miesa mieszanego", SubCategory.PotrawyZMiesaMieszanego},
      {"Potrawy drobiowe", SubCategory.PotrawyDrobiowe},
      {"Lody mleczno-owocowe", SubCategory.LodyMlecznoOwocowe},
      {"Ryby wedzone", SubCategory.RybyWedzone},
      {"Surowki", SubCategory.Surowki},
      {"Produkty sniadaniowe", SubCategory.ProduktySniadaniowe},
      {"Wedliny z kurczaka", SubCategory.WedlinyZKurczaka},
      {"Przystawki", SubCategory.Przystawki},
      {"Produkty obiadowe", SubCategory.ProduktyObiadowe},
      {"Potrawy z grzybow", SubCategory.PotrawyZGrzybow},
      {"Wedliny podrobowe", SubCategory.WedlinyPodrobowe},
      {"Przetwory rybne", SubCategory.PrzetworyRybne},
      {"przetwory warzywne", SubCategory.PrzetworyWarzywne},
      {"Sery twarogowe", SubCategory.SeryTwarogowe},
      {"Soki", SubCategory.Soki},
      {"Wieprzowina", SubCategory.Wieprzowina},
      {"Wina", SubCategory.Wina},
      {"Wodki", SubCategory.Wodki},
      {"Wolowina", SubCategory.Wolowina},
    };
  }
}
