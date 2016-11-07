using System;
using System.Collections.Generic;

namespace Tools
{
  public static class CollectionHelper
  {
    private static readonly Random Random = new Random();

    public static T GetRandomItem<T>(this List<T> collection)
    {
      return collection[Random.Next(collection.Count)];
    }
  }
}
