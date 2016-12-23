﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ConsoleInterface.Storage;

namespace Aggregator
{
  [DataContract]
  public class FrontResult
  {
    [DataMember]
    public double PopulationSize { get; set; }

    [DataMember]
    public double MutationProbability { get; set; }

    [DataMember]
    public double Iterations { get; set; }

    [DataMember]
    public double Time { get; set; }

    [DataMember(Name = "Name")]
    public string SeriesName { get; set; }

    [DataMember]
    public List<ResultPoint> ResultPoints { get; set; }

    public string Machine { get; set; }
  }
}