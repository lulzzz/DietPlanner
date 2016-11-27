﻿using System.Collections.Generic;

namespace DietPlanning.NSGA
{
  public class NsgaLog
  {
    public List<int> FrontsNumberLog;
    public List<ObjectiveLog> ObjectiveLogs;

    public NsgaLog()
    {
      ObjectiveLogs = new List<ObjectiveLog>();
    }
  }

  public class ObjectiveLog
  {
    public ObjectiveType ObjectiveType;
    public double ObjectiveValue;
    public int Iteration;
  }
}