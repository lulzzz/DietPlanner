using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tools
{
  public static class CsvLogger
  {
    private const char Separator = ';';

    public static readonly Dictionary<string, StringBuilder> Loggers = new Dictionary<string, StringBuilder>();

    public static void RegisterLogger(string name)
    {
      Loggers.Add(name, new StringBuilder());
    }

    public static void AddRow(string loggerName, dynamic[] row)
    {
      if (!Loggers.ContainsKey(loggerName))
      {
        RegisterLogger(loggerName);
      }

      foreach (var field in row)
      {
        Loggers[loggerName].Append($"{field.ToString()}{Separator}");
      }
      Loggers[loggerName].Append('\n');
    }

    public static void Write(string loggerName, string filePath)
    {
      if (Loggers.ContainsKey(loggerName))
      {
        File.WriteAllText(filePath, Loggers[loggerName].ToString());
        RemvoeLogger(loggerName);
      }
    }

    public static void RemvoeLogger(string loggerName)
    {
      if (Loggers.ContainsKey(loggerName))
        Loggers.Remove(loggerName);
    }
  }
}
