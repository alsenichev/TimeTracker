using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain;
using Newtonsoft.Json;

namespace DataAccess
{
  public class MainRepository
  {
    private readonly string fileName =
      Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Daily logs",
        "Logs.json");

    public IList<DailyLog> LoadLogs()
    {
      if (File.Exists(fileName))
      {
        string json = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<TimesheetData>(json).Logs;
      }

      return new List<DailyLog>();
    }

    public void SaveLogs(IEnumerable<DailyLog> logs)
    {
      if (!Directory.GetParent(fileName).Exists)
      {
        Directory.GetParent(fileName).Create();
      }
      var data = new TimesheetData {Logs = logs.ToList()};
      string json = JsonConvert.SerializeObject(data);

      // for now, possible exceptions will be handled on the application-level
      File.WriteAllText(fileName, json);
    }
  }
}