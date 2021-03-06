﻿using System.Collections.Generic;

namespace DietPlanning.Core.DomainObjects
{
  public static class GroupsMapping
  {
    public static Dictionary<SubCategory, MainCategory> SubToMainCategoryMapping = new Dictionary<SubCategory, MainCategory>()
      {
        {SubCategory.OwoceSwieze, MainCategory.OwoceIPrzetworyOwocowe},
        {SubCategory.Wypieki, MainCategory.Desery},
        {SubCategory.WedlinyWieprzoweWoloweIMieszane, MainCategory.MiesoIPrzetworyMiesne},
        {SubCategory.Zupy, MainCategory.Zupy},
        {SubCategory.MieszankiWarzywneMrozOne, MainCategory.WarzywaIPrzetworyWarzywne},
        {SubCategory.PotrawyZWolowiny, MainCategory.PotrawyMiesne},
        {SubCategory.PotrawyWarzywnoMiesne, MainCategory.PotrawyWarzywnoMiesne},
        {SubCategory.PotrawyZWieprzowiny, MainCategory.PotrawyMiesne},
        {SubCategory.PotrawyZKaszMakiZiemniakow, MainCategory.PotrawyZKaszMakiZiemniakow},
        {SubCategory.WarzywaGotowane, MainCategory.WarzywaIPrzetworyWarzywne},
        {SubCategory.DeseryMleczne, MainCategory.Desery},
        {SubCategory.MieszankiWarzywneMrozone, MainCategory.WarzywaIPrzetworyWarzywne},
        {SubCategory.PieczywoPszennePolcukiernicze, MainCategory.ProduktyZbozowe},
        {SubCategory.PieczywoPszenne, MainCategory.ProduktyZbozowe},
        {SubCategory.PieczywoMieszane, MainCategory.ProduktyZbozowe},
        {SubCategory.PieczywoPszenneWyborowe, MainCategory.ProduktyZbozowe},
        {SubCategory.Chipsy, MainCategory.Przekaski},
        {SubCategory.PieczywoZytnie, MainCategory.ProduktyZbozowe},
        {SubCategory.Ciastka, MainCategory.Przekaski},
        {SubCategory.PotrawyZCieleciny, MainCategory.PotrawyMiesne},
        {SubCategory.Slodycze, MainCategory.Przekaski},
        {SubCategory.PotrawyRybne, MainCategory.PotrawyRybne},
        {SubCategory.DzemyKonfituryMarmoladyPowidla, MainCategory.OwoceIPrzetworyOwocowe},
        {SubCategory.OwoceSuszone, MainCategory.OwoceIPrzetworyOwocowe},
        {SubCategory.Desery, MainCategory.Desery},
        {SubCategory.PotrawyZJaj, MainCategory.PotrawyZJaj},
        {SubCategory.NapojeMleczne, MainCategory.MlekoIPrzetworyMleczne},
        {SubCategory.Warzywa, MainCategory.WarzywaIPrzetworyWarzywne},
        {SubCategory.ZupyMleczne, MainCategory.Zupy},
        {SubCategory.PotrawyZMiesaMieszanego, MainCategory.PotrawyMiesne},
        {SubCategory.PotrawyDrobiowe, MainCategory.PotrawyMiesne},
        {SubCategory.LodyMlecznoOwocowe, MainCategory.MlekoIPrzetworyMleczne},
        {SubCategory.RybyWedzone, MainCategory.RybyIPrzetworyRybne},
        {SubCategory.Surowki, MainCategory.PotrawyZWarzyw},
        {SubCategory.ProduktySniadaniowe, MainCategory.ProduktyZbozowe},
        {SubCategory.WedlinyZKurczaka, MainCategory.MiesoIPrzetworyMiesne},
        {SubCategory.Przystawki, MainCategory.Przystawki},
        {SubCategory.ProduktyObiadowe, MainCategory.ProduktyZbozowe},
        {SubCategory.PotrawyZGrzybow, MainCategory.PotrawyZWarzyw},
        {SubCategory.WedlinyPodrobowe, MainCategory.MiesoIPrzetworyMiesne},
        {SubCategory.PrzetworyRybne, MainCategory.RybyIPrzetworyRybne},
        {SubCategory.PrzetworyWarzywne, MainCategory.WarzywaIPrzetworyWarzywne},
        {SubCategory.SeryTwarogowe, MainCategory.MlekoIPrzetworyMleczne},
        {SubCategory.Soki, MainCategory.Napoje},
        {SubCategory.Wieprzowina, MainCategory.MiesoIPrzetworyMiesne},
        {SubCategory.Wina, MainCategory.NapojeAlkoholowe},
        {SubCategory.Wodki, MainCategory.NapojeAlkoholowe},
        {SubCategory.Wolowina, MainCategory.MiesoIPrzetworyMiesne},
      };
  }
}
