namespace Storage
{
  public class AverageResult
  {
    public NormalityResult NormalityHv;
    public NormalityResult NormalitySigma;
    public NormalityResult NormalityTime;
    public double Hypervolume;
    public double Time;
    public double Sigma;

    public AverageResult()
    {
      NormalityTime = new NormalityResult();
      NormalitySigma = new NormalityResult();
      NormalityHv = new NormalityResult();
    }
  }
}
