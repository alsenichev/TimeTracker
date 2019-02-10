using System;
using System.Linq;
using DataAccess;
using Domain;
using Domain.BusinessRules;
using Domain.Models;

namespace TimesheetConsole.Commands
{
  static class StartWorkingDay
  {
    private static string WorkingDayStarted(DateTime time) => 
      $"Hello {Program.ProductOwner}. Your working day started {time:f}";

    private static string WorkingDayAlreadyStarted(DateTime time) => 
      $"{Program.ProductOwner}, your working day is already started at {time:f}";

    public static string Execute(DateTime now)
    {
      var repo = new MainRepository();
      var logs = repo.LoadLogs().OrderByDescending(l => l.DayStarted).ToList();
      if (logs.Count > 0 && TimeManagement.IsToday(logs[0].DayStarted))
      {
        return WorkingDayAlreadyStarted(logs[0].DayStarted);
      }

      var log = new DailyLog {DayStarted = now};
      logs.Add(log);
      repo.SaveLogs(logs);
      return WorkingDayStarted(now);
    }
  }
}