using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain;
using Domain.BusinessRules;
using Domain.Models;
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

    public DailyLog LoadLatestLog()
    {
      return LoadLogs().OrderByDescending(l => l.DayStarted).FirstOrDefault();
    }

    public void UpdateTodaysLog(DailyLog log)
    {
      var logs = LoadLogs().OrderByDescending(l => l.DayStarted).ToList();
      if (logs[0] != null && TimeManagement.IsToday(logs[0].DayStarted))
      {
        logs[0] = log;
      }
      else
      {
        logs.Insert(0, log);
      }
      SaveLogs(logs);
    }

    public void SaveLogs(IEnumerable<DailyLog> logs)
    {
      if (!Directory.GetParent(fileName).Exists)
      {
        Directory.GetParent(fileName).Create();
      }
      var data = new TimesheetData { Logs = logs.ToList() };
      string json = JsonConvert.SerializeObject(data);

      File.WriteAllText(fileName, json);
    }
  }
}