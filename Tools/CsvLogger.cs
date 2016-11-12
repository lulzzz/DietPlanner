using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tools
{
  public static class CsvLogger
  {
    private static StringBuilder _csv;
    private const char Separator = ';';

    public static void Init()
    {
      _csv = new StringBuilder();
    }

    public static void AddRow(dynamic[] row)
    {
      foreach (var field in row)
      {
        _csv.Append($"{field.ToString()}{Separator}");
      }
      _csv.Append('\n');
    }

    public static void Write(string filePath)
    {
      File.WriteAllText(filePath, _csv.ToString());
    }
  }
}
