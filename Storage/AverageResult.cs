namespace Storage
{
  public class AverageResult
  {
    public NormalityResult NormalityHv;
    public NormalityResult NormalityEpsilon;
    public NormalityResult NormalityTime;
    public double Hypervolume;
    public double Time;
    public double Epsilon;

    public AverageResult()
    {
      NormalityTime = new NormalityResult();
      NormalityEpsilon = new NormalityResult();
      NormalityHv = new NormalityResult();
    }
  }
}
