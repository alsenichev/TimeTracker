using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataAccess.Mappers;
using DataAccess.Models;
using Domain.Models;
using Domain.Utils;
using Newtonsoft.Json;

namespace DataAccess
{
  public class MainRepository
  {
#if DEBUG
    private readonly string fileName =
      Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "Daily logs",
        "Days.json");
#else
    private readonly string fileName =
      Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Dropbox",
        "LanitTercom",
        "Timesheet",
        "Days.json");
#endif

    #region private methods
    private Result<LogBook> GetLogBook()
    {
      try
      {
        if (File.Exists(fileName))
        {
          string json = File.ReadAllText(fileName);
          LogBookData result =  JsonConvert.DeserializeObject<LogBookData>(json);
          return Results.Success(LogBookMapper.SafeMap(result));
        }
        return Results.Success(LogBookMapper.SafeMap(null));
      }
      catch (Exception e)
      {
        return Results.Failure<LogBook>($"Failed to read daily sheets from disk: {e}.");
      }
    }
    private Result<object> SaveLogBook(LogBook logBook)
    {
      try
      {
        if (!Directory.GetParent(fileName).Exists)
        {
          Directory.GetParent(fileName).Create();
        }
        var data = new LogBookData
        {
          // save only for 2 latest months
          Days = logBook.Days.OrderByDescending(l => l.DayStarted)
            .Take(61).Select(d=>DayMapper.Map(d)).ToList(),
          Stash = logBook.Stash
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

    private Result<Status> OfNewDay(LogBook logBook)
    {
      // stash becomes a deposit for a new day
      var day = new Day(DateTime.Now.RoundSeconds(), TimeSpan.Zero, logBook.Stash, new List<TaskEntry>());
      var days = logBook.Days;
      days.Add(day);
      // reset stash after it was converted into the deposit for a new day
      var toSave = new LogBook(days, TimeSpan.Zero);
      // don't like the idea to recursively call GetStatus(), because if we
      // might save log and mistakenly read it from different places, we'll get an infinite recursion
      return SaveLogBook(toSave).Map(_ => new Status(day, TimeSpan.Zero));
    }
    #endregion

    public Result<Status> GetStatus()
    {
      Option<Day> latest (LogBook b) =>
        b.Days.Any()
          ? Options.Some(b.Days.OrderByDescending(s => s.DayStarted).First())
          : Options.None<Day>();
      Option<Day> today(Day s) =>
        Options.Of(s, i => i.DayStarted.Date.Equals(DateTime.Now.Date));
      return GetLogBook()
        .Bind(b =>
          latest(b)
            .Bind(today)
            .Fold(t =>
              Results.Success(new Status(t, b.Stash)),
              () => OfNewDay(b)));
    }

    /// <summary>
    /// This will add or update existing day if it is today. And save.
    /// </summary>
    public Result<object> SaveTodaySheet(Day day)
    {
      var logBook = GetLogBook();
      Result<List<Day>> updatedDays = logBook
        .Map(b => b.Days)
        .Map(s => s.ToDictionary(k => k.DayStarted.Date))
        .Map(d =>
            {
              d[day.DayStarted.Date] = day;
              return d.Values.ToList();
            });
      Result<object> saveLogBook(TimeSpan stash) =>
        updatedDays.Bind(days => SaveLogBook(new LogBook(days, stash)));
      return logBook.Map(d => d.Stash).Bind(saveLogBook);
    }

    public Result<object> SaveStash(TimeSpan stash)
    {
      return GetLogBook().Bind(b => SaveLogBook(new LogBook(b.Days, stash)));
    }

    public Result<IList<Day>> GetAllDays()
    {
      return GetLogBook().Map(b => b.Days);
    }
  }
}