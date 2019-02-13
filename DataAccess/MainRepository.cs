using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain;
using Domain.BusinessRules;
using Domain.Models;
using Domain.Utils;
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

    #region private methods
    public Result<object> SaveSheets(IEnumerable<DailySheet> logs)
    {
      try
      {
        if (!Directory.GetParent(fileName).Exists)
        {
          Directory.GetParent(fileName).Create();
        }
        var data = new TimesheetData
        {
          // save only for 2 latest months
          Logs = logs.OrderByDescending(l => l.DayStarted).Take(61).ToList()
        };
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(fileName, json);
        return Results.SuccessfulUnit;
      }
      catch (Exception e)
      {
        return Results.Failure<object>($"Failed to save sheets: {e.Message}.");
      }
    }
    private Result<Option<DailySheet>> GetLatestSheet()
    {
      return GetAllSheets().Map(
        r => Options.NotNull(
          r.OrderByDescending(s => s.DayStarted).FirstOrDefault()));
    }
    #endregion

    public Result<IList<DailySheet>> GetAllSheets()
    {
      try
      {
        if (File.Exists(fileName))
        {
          string json = File.ReadAllText(fileName);
          IList<DailySheet> result =  JsonConvert.DeserializeObject<TimesheetData>(json)?.Logs;
          return Results.Success(result ?? new List<DailySheet>());
        }
        return Results.Success<IList<DailySheet>>(new List<DailySheet>());
      }
      catch (Exception e)
      {
        return Results.Failure<IList<DailySheet>>($"Failed to read sheets from disk: {e}.");
      }
    }

    public Result<Option<DailySheet>> GetTodaySheet()
    {
      Result<Option<DailySheet>> maybeLatestResult = GetLatestSheet();
      Option<DailySheet> maybeToday(DailySheet s) =>
        Options.Of(s, i => TimeManagement.IsToday(i.DayStarted));
      return maybeLatestResult.Map(o => o.Bind(maybeToday));
    }

    /// <summary>
    /// This will add or update existing sheet if it is today. And save.
    /// </summary>
    public Result<object> SaveTodaySheet(DailySheet sheet)
    {
      var all = GetAllSheets();
      var dict = all.Map(s => s.ToDictionary(k => k.DayStarted));
      var update = dict.Map(d =>
          {
            d[sheet.DayStarted] = sheet;
            return d.Values;
          });
      return update.Bind(SaveSheets);
    }
  }
}